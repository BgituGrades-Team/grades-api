using AutoMapper;
using BgituGrades.DTO;
using BgituGrades.Entities;
using BgituGrades.Models.Work;
using BgituGrades.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BgituGrades.Services
{
    public interface IWorkService
    {
        Task<List<WorkResponse>> GetAllWorksAsync(CancellationToken cancellationToken);
        Task<WorkResponse> CreateWorkAsync(CreateWorkRequest request, CancellationToken cancellationToken);
        Task<WorkResponse?> GetWorkByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> UpdateWorkAsync(UpdateWorkRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteWorkAsync(int id, CancellationToken cancellationToken);
    }
    public class WorkService(IWorkRepository workRepository, IMapper mapper, IDistributedCache cache) : IWorkService
    {
        private readonly IWorkRepository _workRepository = workRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IDistributedCache _cache = cache;
        private const string AllWorksKey = "work:all:works";

        public async Task<WorkResponse> CreateWorkAsync(CreateWorkRequest request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Work>(request);
            var createdEntity = await _workRepository.CreateWorkAsync(entity, cancellationToken: cancellationToken);

            await _cache.RemoveAsync(AllWorksKey);
            return _mapper.Map<WorkResponse>(createdEntity);
        }

        public async Task<bool> DeleteWorkAsync(int id, CancellationToken cancellationToken)
        {
            var result = await _workRepository.DeleteWorkAsync(id, cancellationToken: cancellationToken);
            if (result)
            {
                await _cache.RemoveAsync(AllWorksKey);
            }
            return result;
        }

        public async Task<WorkResponse?> GetWorkByIdAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _workRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
            return entity == null ? null : _mapper.Map<WorkResponse>(entity);
        }

        public async Task<List<WorkResponse>> GetAllWorksAsync(CancellationToken cancellationToken)
        {
            var cached = await GetFromCacheAsync<List<WorkResponse>>(AllWorksKey);
            if (cached != null)
                return cached;

            var entities = await _workRepository.GetAllWorksAsync(cancellationToken: cancellationToken);
            var result = _mapper.Map<List<WorkResponse>>(entities).ToList();
            await SetCacheAsync(AllWorksKey, result, TimeSpan.FromHours(4));
            return result;
        }

        public async Task<bool> UpdateWorkAsync(UpdateWorkRequest request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Work>(request);
            var result = await _workRepository.UpdateWorkAsync(entity, cancellationToken: cancellationToken);
            if (result)
            {
                await _cache.RemoveAsync(AllWorksKey);
            }
            return result;
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
    }

}
