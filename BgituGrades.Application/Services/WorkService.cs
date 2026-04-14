using AutoMapper;
using BgituGrades.Application.Caching;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Interfaces;
using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Interfaces;
using Microsoft.Extensions.Caching.Hybrid;

namespace BgituGrades.Application.Services
{
    
    public class WorkService(IWorkRepository workRepository, IMapper mapper, ICacheService cacheService) : IWorkService
    {
        private readonly IWorkRepository _workRepository = workRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ICacheService _cacheService = cacheService;

        private static readonly HybridCacheEntryOptions DefaultOptions = new()
        {
            Expiration = TimeSpan.FromMinutes(10),
            LocalCacheExpiration = TimeSpan.FromMinutes(2)
        };

        public async Task<WorkDTO> CreateWorkAsync(WorkDTO work, CancellationToken cancellationToken)
        {
            work.IssuedDate = DateOnly.FromDateTime(DateTime.UtcNow);
            var entity = _mapper.Map<Work>(work);
            var createdEntity = await _workRepository.CreateWorkAsync(entity, cancellationToken: cancellationToken);

            await _cacheService.RemoveAsync(CacheKeys.WorkAll(), ct: cancellationToken);
            return _mapper.Map<WorkDTO>(createdEntity);
        }

        public async Task<bool> DeleteWorkAsync(int id, CancellationToken cancellationToken)
        {
            var result = await _workRepository.DeleteWorkAsync(id, cancellationToken: cancellationToken);
            if (result)
                await _cacheService.RemoveAsync(CacheKeys.WorkAll(), ct: cancellationToken);
            return result;
        }

        public async Task<WorkDTO?> GetWorkByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _cacheService
                .GetOrCreateAsync(
                    key: CacheKeys.Work(id),
                    factory: async token =>
                    {
                        var entity = await _workRepository.GetByIdAsync(id, cancellationToken: token);
                        return _mapper.Map<WorkDTO>(entity);
                    },
                    options: DefaultOptions, ct: cancellationToken);
        }

        public async Task<List<WorkDTO>> GetAllWorksAsync(CancellationToken cancellationToken)
        {
            return await _cacheService
                .GetOrCreateAsync(
                    key: CacheKeys.WorkAll(),
                    factory: async token =>
                    {
                        var entities = await _workRepository.GetAllWorksAsync(cancellationToken: token);
                        return _mapper.Map<List<WorkDTO>>(entities);
                    },
                    options: DefaultOptions, ct: cancellationToken);
        }

        public async Task<WorkDTO> UpdateWorkAsync(WorkDTO work, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Work>(work);
            var updatedEntity = await _workRepository.UpdateWorkAsync(entity, cancellationToken: cancellationToken);
            if (updatedEntity != null)
                await _cacheService.RemoveAsync(CacheKeys.WorkAll(), ct: cancellationToken);

            var result = _mapper.Map<WorkDTO>(updatedEntity);
            return result;
        }
    }
}
