using AutoMapper;
using BgituGrades.DTO;
using BgituGrades.Entities;
using BgituGrades.Models.Group;
using BgituGrades.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BgituGrades.Services
{
    public interface IGroupService
    {
        Task<IEnumerable<GroupResponse>> GetGroupsByDisciplineAsync(int disciplineId);
        Task<IEnumerable<GroupResponse>> GetAllAsync();
        Task<GroupResponse> CreateGroupAsync(CreateGroupRequest request);
        Task<GroupResponse?> GetGroupByIdAsync(int id);
        Task<bool> UpdateGroupAsync(UpdateGroupRequest request);
        Task<bool> DeleteGroupAsync(int id);
        Task<IEnumerable<GroupDTO>> GetAllGroupsDtoAsync();
        Task<IEnumerable<GroupDTO>> GetGroupsDtoByDisciplineAsync(int disciplineId);
        Task<GroupDTO?> GetGroupDtoByIdAsync(int id);
    }

    public class GroupService(IGroupRepository groupRepository, IMapper mapper, IDistributedCache cache) : IGroupService
    {
        private readonly IGroupRepository _groupRepository = groupRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IDistributedCache _cache = cache;
        private const string AllGroupsKey = "group:all";
        private const string GroupsByDisciplineKey = "group:discipline:";

        public async Task<GroupResponse> CreateGroupAsync(CreateGroupRequest request)
        {
            var entity = _mapper.Map<Group>(request);
            var createdEntity = await _groupRepository.CreateGroupAsync(entity);
            // Инвалидировать кэш
            await _cache.RemoveAsync(AllGroupsKey);
            return _mapper.Map<GroupResponse>(createdEntity);
        }

        public async Task<bool> DeleteGroupAsync(int id)
        {
            var result = await _groupRepository.DeleteGroupAsync(id);
            if (result)
            {
                await _cache.RemoveAsync(AllGroupsKey);
            }
            return result;
        }

        public async Task<IEnumerable<GroupResponse>> GetAllAsync()
        {
            // ✅ Кэш всех групп
            var cached = await GetFromCacheAsync<IEnumerable<GroupResponse>>(AllGroupsKey);
            if (cached != null)
                return cached;

            var groups = await _groupRepository.GetAllAsync();
            var response = _mapper.Map<IEnumerable<GroupResponse>>(groups).ToList();
            await SetCacheAsync(AllGroupsKey, response, TimeSpan.FromHours(2));
            return response;
        }

        public async Task<GroupResponse?> GetGroupByIdAsync(int id)
        {
            var entity = await _groupRepository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<GroupResponse>(entity);
        }

        public async Task<IEnumerable<GroupResponse>> GetGroupsByDisciplineAsync(int disciplineId)
        {
            // ✅ Кэш групп по дисциплинам
            var cacheKey = $"{GroupsByDisciplineKey}{disciplineId}";
            var cached = await GetFromCacheAsync<IEnumerable<GroupResponse>>(cacheKey);
            if (cached != null)
                return cached;

            var entities = await _groupRepository.GetGroupsByDisciplineAsync(disciplineId);
            var result = _mapper.Map<IEnumerable<GroupResponse>>(entities).ToList();
            await SetCacheAsync(cacheKey, result, TimeSpan.FromHours(2));
            return result;
        }

        public async Task<bool> UpdateGroupAsync(UpdateGroupRequest request)
        {
            var entity = _mapper.Map<Group>(request);
            var result = await _groupRepository.UpdateGroupAsync(entity);
            if (result)
            {
                await _cache.RemoveAsync(AllGroupsKey);
            }
            return result;
        }

        public async Task<IEnumerable<GroupDTO>> GetAllGroupsDtoAsync()
        {
            var groups = await _groupRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<GroupDTO>>(groups);
        }

        public async Task<IEnumerable<GroupDTO>> GetGroupsDtoByDisciplineAsync(int disciplineId)
        {
            var entities = await _groupRepository.GetGroupsByDisciplineAsync(disciplineId);
            return _mapper.Map<IEnumerable<GroupDTO>>(entities);
        }

        public async Task<GroupDTO?> GetGroupDtoByIdAsync(int id)
        {
            var entity = await _groupRepository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<GroupDTO>(entity);
        }

        // 🔧 Вспомогательные методы для работы с кэшем
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
                // Логировать ошибку кэширования, но не прерывать выполнение
            }
        }
    }

}