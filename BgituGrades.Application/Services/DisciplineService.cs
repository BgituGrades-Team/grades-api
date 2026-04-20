using AutoMapper;
using BgituGrades.Application.Caching;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Interfaces;
using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Interfaces;
using Microsoft.Extensions.Caching.Hybrid;

namespace BgituGrades.Application.Services
{
    
    public class DisciplineService(IDisciplineRepository disciplineRepository, IMapper mapper, ICacheService cacheService) : IDisciplineService
    {
        private readonly IDisciplineRepository _disciplineRepository = disciplineRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ICacheService _cacheService = cacheService;
       

        private static readonly HybridCacheEntryOptions DefaultOptions = new()
        {
            Expiration = TimeSpan.FromMinutes(10),
            LocalCacheExpiration = TimeSpan.FromMinutes(5)
        };

        public async Task<DisciplineDTO> CreateDisciplineAsync(DisciplineDTO disciplineDto, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Discipline>(disciplineDto);
            var createdEntity = await _disciplineRepository.CreateDisciplineAsync(entity, cancellationToken: cancellationToken);
            await _cacheService.RemoveAsync(CacheKeys.DisicplineAll(), cancellationToken);
            return _mapper.Map<DisciplineDTO>(createdEntity);
        }

        public async Task<List<DisciplineDTO>> CreateDisciplineAsync(IEnumerable<DisciplineDTO> disciplineDto, CancellationToken cancellationToken)
        {
            var entities = _mapper.Map<List<Discipline>>(disciplineDto);
            var createdEntities = await _disciplineRepository.CreateDisciplineAsync(entities, cancellationToken: cancellationToken);
            await _cacheService.RemoveAsync(CacheKeys.DisicplineAll(), cancellationToken);
            return _mapper.Map<List<DisciplineDTO>>(createdEntities);
        }

        public async Task<bool> DeleteDisciplineAsync(int id, CancellationToken cancellationToken)
        {
            var result = await _disciplineRepository.DeleteDisciplineAsync(id, cancellationToken: cancellationToken);
            if (result)
            {
                await _cacheService.RemoveAsync(CacheKeys.DisicplineAll(), cancellationToken);
            }
            return result;
        }

        public async Task<List<DisciplineDTO>> GetAllDisciplinesAsync(CancellationToken cancellationToken)
        {
            return await _cacheService.GetOrCreateAsync(
                key: CacheKeys.DisicplineAll(),
                factory: async token =>
                {
                    var entities = await _disciplineRepository.GetAllAsync(cancellationToken: token);
                    return _mapper.Map<List<DisciplineDTO>>(entities);
                }, options: DefaultOptions, ct: cancellationToken);
        }

        public async Task<List<DisciplineDTO>> GetDisciplineByGroupIdAsync(IEnumerable<int> groupIds, CancellationToken cancellationToken)
        {
            var results = new List<DisciplineDTO>();
            var missingIds = new List<int>();

            foreach (var id in groupIds)
            {
                var singleCacheKey = CacheKeys.DisciplineByGroup(id);
                var cached = await _cacheService.GetOrCreateAsync<List<DisciplineDTO>?>(
                    key: singleCacheKey,
                    factory: _ => ValueTask.FromResult<List<DisciplineDTO>?>(null),
                    options: DefaultOptions,
                    ct: cancellationToken);

                if (cached != null)
                    results.AddRange(cached);
                else
                    missingIds.Add(id);
            }

            if (missingIds.Count != 0)
            {
                var entities = await _disciplineRepository.GetByGroupIdsAsync([.. missingIds], cancellationToken);
                if (entities != null && entities.Count != 0)
                {
                    foreach (var groupId in missingIds)
                    {
                        var disciplinesForGroup = entities
                            .Where(d => d.Classes != null && d.Classes.Any(c => c.GroupId == groupId))
                            .ToList();

                        var mappedDisciplines = _mapper.Map<List<DisciplineDTO>>(disciplinesForGroup);

                        await _cacheService.GetOrCreateAsync(
                            key: CacheKeys.DisciplineByGroup(groupId),
                            factory: _ => ValueTask.FromResult(mappedDisciplines),
                            options: DefaultOptions,
                            ct: cancellationToken);

                        results.AddRange(mappedDisciplines);
                    }
                }
            }

            return results.DistinctBy(d => d.Id).ToList();
        }

        public async Task<List<DisciplineDTO>> GetArchivedDisciplinesByGroupIdsAsync(IEnumerable<int> groupIds, CancellationToken cancellationToken)
        {
            var disciplines = await _disciplineRepository.GetArchivedByGroupIdsAsync(groupIds, cancellationToken);
            var results = _mapper.Map<List<DisciplineDTO>>(disciplines);
            return results;
        }
    }
}
