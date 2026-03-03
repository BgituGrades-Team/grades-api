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
        Task<IEnumerable<WorkResponse>> GetAllWorksAsync();
        Task<WorkResponse> CreateWorkAsync(CreateWorkRequest request);
        Task<WorkResponse?> GetWorkByIdAsync(int id);
        Task<bool> UpdateWorkAsync(UpdateWorkRequest request);
        Task<bool> DeleteWorkAsync(int id);
        Task<IEnumerable<WorkDTO>> GetAllWorksDtoAsync();
        Task<WorkDTO?> GetWorkDtoByIdAsync(int id);
    }
    public class WorkService(IWorkRepository workRepository, IMapper mapper, IDistributedCache cache) : IWorkService
    {
        private readonly IWorkRepository _workRepository = workRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IDistributedCache _cache = cache;
        private const string AllWorksKey = "work:all:works";

        public async Task<WorkResponse> CreateWorkAsync(CreateWorkRequest request)
        {
            var entity = _mapper.Map<Work>(request);
            var createdEntity = await _workRepository.CreateWorkAsync(entity);
            // Инвалидировать кэш
            await _cache.RemoveAsync(AllWorksKey);
            return _mapper.Map<WorkResponse>(createdEntity);
        }

        public async Task<bool> DeleteWorkAsync(int id)
        {
            var result = await _workRepository.DeleteWorkAsync(id);
            if (result)
            {
                await _cache.RemoveAsync(AllWorksKey);
            }
            return result;
        }

        public async Task<WorkResponse?> GetWorkByIdAsync(int id)
        {
            var entity = await _workRepository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<WorkResponse>(entity);
        }

        public async Task<IEnumerable<WorkResponse>> GetAllWorksAsync()
        {
            // ✅ Кэш всех работ (справочные данные - редко меняются)
            var cached = await GetFromCacheAsync<IEnumerable<WorkResponse>>(AllWorksKey);
            if (cached != null)
                return cached;

            var entities = await _workRepository.GetAllWorksAsync();
            var result = _mapper.Map<IEnumerable<WorkResponse>>(entities).ToList();
            await SetCacheAsync(AllWorksKey, result, TimeSpan.FromHours(4));
            return result;
        }

        public async Task<bool> UpdateWorkAsync(UpdateWorkRequest request)
        {
            var entity = _mapper.Map<Work>(request);
            var result = await _workRepository.UpdateWorkAsync(entity);
            if (result)
            {
                await _cache.RemoveAsync(AllWorksKey);
            }
            return result;
        }

        public async Task<IEnumerable<WorkDTO>> GetAllWorksDtoAsync()
        {
            var entities = await _workRepository.GetAllWorksAsync();
            return _mapper.Map<IEnumerable<WorkDTO>>(entities);
        }

        public async Task<WorkDTO?> GetWorkDtoByIdAsync(int id)
        {
            var entity = await _workRepository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<WorkDTO>(entity);
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
