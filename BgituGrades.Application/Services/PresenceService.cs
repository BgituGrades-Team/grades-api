using AutoMapper;
using BgituGrades.Application.Interfaces;
using BgituGrades.Application.Models.Class;
using BgituGrades.Application.Models.Presence;
using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BgituGrades.Application.Services
{
    
    public class PresenceService(IPresenceRepository presenceRepository, IMapper mapper, IDistributedCache cache, ICacheService cacheService) : IPresenceService
    {
        private readonly IPresenceRepository _presenceRepository = presenceRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IDistributedCache _cache = cache;
        private const string CacheKeyPrefix = "presence:";
        private const string AllPresencesKey = "presence:all";


        public async Task<PresenceResponse> CreatePresenceAsync(CreatePresenceRequest request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Presence>(request);
            var createdEntity = await _presenceRepository.CreatePresenceAsync(entity, cancellationToken: cancellationToken);

            await InvalidateCacheAsync(request.DisciplineId, request.StudentId);
            return _mapper.Map<PresenceResponse>(createdEntity);
        }

        public async Task<List<PresenceResponse>> GetAllPresencesAsync(CancellationToken cancellationToken)
        {

            var cached = await GetFromCacheAsync<List<PresenceResponse>>(AllPresencesKey);
            if (cached != null)
                return cached;

            var entities = await _presenceRepository.GetAllPresencesAsync(cancellationToken: cancellationToken);
            var result = _mapper.Map<List<PresenceResponse>>(entities).ToList();
            await SetCacheAsync(AllPresencesKey, result, TimeSpan.FromHours(1));
            return result;
        }

        public async Task<List<PresenceResponse>> GetPresencesByDisciplineAndGroupAsync(GetPresenceByDisciplineAndGroupRequest request, CancellationToken cancellationToken)
        {

            var cacheKey = $"{CacheKeyPrefix}discipline:{request.DisciplineId}:group:{request.GroupId}";

            var cached = await GetFromCacheAsync<List<PresenceResponse>>(cacheKey);
            if (cached != null)
                return cached;

            var entities = await _presenceRepository.GetPresencesByDisciplineAndGroupAsync(request.DisciplineId, request.GroupId, cancellationToken: cancellationToken);
            var result = _mapper.Map<List<PresenceResponse>>(entities).ToList();
            await SetCacheAsync(cacheKey, result, TimeSpan.FromHours(2));
            return result;
        }

        public async Task<bool> DeletePresenceByStudentAndDateAsync(DeletePresenceByStudentAndDateRequest request, CancellationToken cancellationToken)
        {
            var result = await _presenceRepository.DeletePresenceByStudentAndDateAsync(request.StudentId, request.Date, cancellationToken: cancellationToken);
            if (result)
            {
                await InvalidateCacheAsync(0, request.StudentId);
            }
            return result;
        }

        public async Task UpdatePresenceAsync(UpdatePresenceRequest request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Presence>(request);
            await _presenceRepository.UpdatePresenceAsync(entity, cancellationToken: cancellationToken);
        }

        public async Task<FullGradePresenceResponse> UpdateOrCreatePresenceAsync(UpdatePresenceGradeRequest request, CancellationToken cancellationToken)
        {
            var updatedPresence = _mapper.Map<Presence>(request);
            var presence = await _presenceRepository.GetAsync(updatedPresence, cancellationToken: cancellationToken);

            if (presence != null)
            {
                presence.IsPresent = request.IsPresent;
                await _presenceRepository.UpdatePresenceAsync(presence, cancellationToken: cancellationToken);
            }
            else
            {
                presence = _mapper.Map<Presence>(request);
                await _presenceRepository.CreatePresenceAsync(presence, cancellationToken: cancellationToken);
            }

            var response = new FullGradePresenceResponse
            {
                StudentId = presence.StudentId,
                Presences = [new GradePresenceResponse {
                    ClassId = presence.ClassId,
                    IsPresent = presence.IsPresent,
                    Date = presence.Date
                }]
            };
            return response;
        }

        public async Task<(int present, int total, TimeOnly StartTime, string DisciplineName, DateOnly Date)?> GetPresenceCountAsync(
            string groupName, string disciplineName,
            DateOnly date, TimeOnly startTime, 
            CancellationToken cancellationToken)
        {
            return await _presenceRepository.GetPresenceCountAsync(groupName, disciplineName, date, startTime, cancellationToken: cancellationToken);
        }

        public async Task<(int present, int total, string GroupKey, TimeOnly StartTime, string DisciplineName, DateOnly Date)?> GetPresenceCountByClassAsync(
            int classId, DateOnly date,
            CancellationToken cancellationToken)
        {
            return await _presenceRepository.GetPresenceCountByClassAsync(classId, date, cancellationToken: cancellationToken);
        }


        private async Task<T?> GetFromCacheAsync<T>(string key)
        {
            try
            {
                var value = await _cache.GetStringAsync(key);
                if (value == null)
                    return default;
                return JsonSerializer.Deserialize<T>(value);
            }
            catch
            {
                return default;
            }
        }

        private async Task SetCacheAsync<T>(string key, T value, TimeSpan expiration)
        {
            try
            {
                var serialized = JsonSerializer.Serialize(value);
                var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration };
                await _cache.SetStringAsync(key, serialized, options);
            }
            catch
            {

            }
        }

        private async Task InvalidateCacheAsync(int disciplineId, int studentId)
        {
            try
            {
                await _cache.RemoveAsync(AllPresencesKey);
                if (disciplineId > 0)
                    await _cache.RemoveAsync($"{CacheKeyPrefix}discipline:{disciplineId}:*");
            }
            catch
            {

            }
        }
    }
}
