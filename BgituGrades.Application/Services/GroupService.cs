using AutoMapper;
using BgituGrades.Application.Caching;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Interfaces;
using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Interfaces;
using BgituGrades.Infrastructure.Features;
using Microsoft.Extensions.Caching.Hybrid;

namespace BgituGrades.Application.Services
{
    

    public class GroupService(IGroupRepository groupRepository, IMapper mapper, ICacheService cacheService) : IGroupService
    {
        private readonly IGroupRepository _groupRepository = groupRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ICacheService _cacheService = cacheService;

        private static readonly HybridCacheEntryOptions DefaultOptions = new()
        {
            Expiration = TimeSpan.FromMinutes(30),
            LocalCacheExpiration = TimeSpan.FromMinutes(10)
        };

        public async Task<GroupDTO> CreateGroupAsync(GroupDTO group, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Group>(group);
            entity.CourseNumber = GroupCourseParser.Parse(entity.Name);
            var createdEntity = await _groupRepository.CreateGroupAsync(entity, cancellationToken: cancellationToken);

            await _cacheService.RemoveByTagAsync(CacheTags.Group(), ct: cancellationToken);
            return _mapper.Map<GroupDTO>(createdEntity);
        }

        public async Task<List<GroupDTO>> CreateGroupAsync(IEnumerable<GroupDTO> groups, CancellationToken cancellationToken)
        {
            var entities = _mapper.Map<List<Group>>(groups);
            foreach (var entity in entities)
                entity.CourseNumber = GroupCourseParser.Parse(entity.Name);

            var createdEntities = await _groupRepository.CreateGroupAsync(entities, cancellationToken: cancellationToken);
            await _cacheService.RemoveByTagAsync(CacheTags.Group(), ct: cancellationToken);
            return _mapper.Map<List<GroupDTO>>(createdEntities);
        }

        public async Task<bool> DeleteGroupAsync(int id, CancellationToken cancellationToken)
        {
            var result = await _groupRepository.DeleteGroupAsync(id, cancellationToken: cancellationToken);
            if (result)
                await _cacheService.RemoveByTagAsync(CacheTags.Group(), ct: cancellationToken);
            return result;
        }

        public async Task<List<GroupDTO>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _cacheService.GetOrCreateAsync(
                key: CacheKeys.GroupAll(),
                factory: async token =>
                {
                    var entities = await _groupRepository.GetAllAsync(cancellationToken: token);
                    return _mapper.Map<List<GroupDTO>>(entities);
                }, 
                tags: CacheTags.GroupAll(),
                options: DefaultOptions, ct: cancellationToken);
        }

        public async Task<GroupDTO?> GetGroupByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _cacheService.GetOrCreateAsync(
                key: CacheKeys.Group(id),
                factory: async token =>
                {
                    var entity = await _groupRepository.GetByIdAsync(id, cancellationToken: token);
                    return entity == null ? null : _mapper.Map<GroupDTO>(entity);
                }, 
                tags: CacheTags.GroupAll(),
                options: DefaultOptions, ct: cancellationToken);
        }

        public async Task<List<GroupDTO>> GetGroupsByDisciplineAsync(int disciplineId, CancellationToken cancellationToken)
        {
            var entities = await _groupRepository.GetGroupsByDisciplineAsync(disciplineId, cancellationToken: cancellationToken);
            var result = _mapper.Map<List<GroupDTO>>(entities);
            return result;
        }

        public async Task<List<GroupDTO>> GetArchivedGroupsByPeriodAsync(int semester, int year, CancellationToken cancellationToken)
        {
            return await _cacheService.GetOrCreateAsync(
                key: CacheKeys.GroupByPeriod(year, semester),
                factory: async token =>
                {
                    var entities = await _groupRepository.GetArchivedByPeriod(semester, year, cancellationToken: token);
                    return _mapper.Map<List<GroupDTO>>(entities);
                },  
                tags: CacheTags.GroupAll(), 
                options: DefaultOptions, ct: cancellationToken);
        }

        public async Task<List<int>> GetCoursesAsync(CancellationToken cancellationToken)
        {
            return await _groupRepository.GetCoursesAsync(cancellationToken);
        }

        public async Task<List<int>> GetArchivedCoursesByPeriodAsync(int year, int semester, CancellationToken cancellationToken)
        {
            return await _groupRepository.GetArchivedCoursesByPeriodAsync(year, semester, cancellationToken);
        }

        public async Task<List<GroupDTO>> GetGroupsByCoursesAsync(IEnumerable<int> courses, CancellationToken cancellationToken)
        {
            var results = new List<GroupDTO>();

            var entities = await _groupRepository.GetGroupsByCoursesAsync([.. courses], cancellationToken);
            if (entities != null && entities.Count != 0)
            {
                foreach (var course in courses)
                {
                    var groupsForCourse = entities
                        .Where(g => g.CourseNumber == course)
                        .ToList();

                    var mapped = _mapper.Map<List<GroupDTO>>(groupsForCourse);
                    results.AddRange(mapped);
                }
            }

            return results.DistinctBy(g => g.Id).ToList();
        }
    }
}