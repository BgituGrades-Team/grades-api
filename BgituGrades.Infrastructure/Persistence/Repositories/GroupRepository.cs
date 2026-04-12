using BgituGrades.Application.Models.Group;
using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Interfaces;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace BgituGrades.Infrastructure.Persistence.Repositories
{
    

    public class GroupRepository(IDbContextFactory<AppDbContext> contextFactory) : IGroupRepository
    {
        public async Task<Group> CreateGroupAsync(Group entity, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            await context.Groups.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<List<Group>> CreateGroupAsync(IEnumerable<Group> entities, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var bulkConfig = new BulkConfig { SetOutputIdentity = true };
            await context.BulkInsertAsync(entities, bulkConfig, cancellationToken: cancellationToken);
            return entities.ToList();
        }

        public async Task DeleteAllAsync(CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            await context.Groups.ExecuteDeleteAsync(cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteGroupAsync(int id, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var result = await context.Groups
                .Where(g => g.Id == id)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
            return result > 0;
        }

        public async Task<List<Group>> GetAllAsync(CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var groups = await context.Groups
                .Include(g => g.Classes!)
                    .ThenInclude(c => c.Discipline)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync(cancellationToken: cancellationToken);
            return groups;
        }

        public async Task<List<Group>> GetArchivedByPeriod(int year, int semester, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var archivedGroups = await context.ReportSnapshots
                .AsNoTracking()
                .Where(r => r.Semester == semester && r.Year == year)
                .Select(r => new { r.GroupId, r.GroupName })
                .Distinct()
                .Select(r => new Group { Id = r.GroupId, Name = r.GroupName })
                .ToListAsync(cancellationToken: cancellationToken);
            return archivedGroups;
        }

        public async Task<List<int>> GetArchivedCoursesByPeriodAsync(int year, int semester, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            return await context.ReportSnapshots
                .AsNoTracking()
                .Where(r => r.Year == year && r.Semester == semester)
                .Select(r => r.GroupCourseNumber)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Group>> GetArchivedGroupsByCoursesAndPeriodAsync(IEnumerable<int> courses, int year, int semester, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);

            return await context.ReportSnapshots
                .AsNoTracking()
                .Where(r => r.Year == year
                         && r.Semester == semester
                         && courses.Contains(r.GroupCourseNumber))
                .GroupBy(r => new { r.GroupId, r.GroupName })
                .Select(g => new Group { Id = g.Key.GroupId, Name = g.Key.GroupName })
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Group>> GetArchivedGroupsByCoursesAsync(IEnumerable<int> courses, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            return await context.ReportSnapshots
                .AsNoTracking()
                .Where(r => courses.Contains(r.GroupCourseNumber))
                .DistinctBy(r => r.GroupId)
                .Select(r => new Group { Id = r.GroupId, Name = r.GroupName })
                .ToListAsync(cancellationToken);
        }

        public async Task<Group?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var entity = await context.Groups.FindAsync([id], cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<List<Group>> GetByIdsAsync(IEnumerable<int> groupIds, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            return await context.Groups
                .Where(g => groupIds.Contains(g.Id))
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<int>> GetCoursesAsync(CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var courses = await context.Groups
                .AsNoTracking()
                .Select(g => g.CourseNumber)
                .Distinct()
                .ToListAsync(cancellationToken);
            return courses;
        }

        public async Task<List<Group>> GetGroupsByCoursesAsync(IEnumerable<int> courses, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var entities = await context.Groups
                .Where(g => courses.Contains(g.CourseNumber))
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return entities;
        }

        public async Task<List<Group>> GetGroupsByDisciplineAsync(int disciplineId, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var entities = await context.Groups
                .Where(g => g.Classes!.Any(c => c.DisciplineId == disciplineId))
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
            return entities;
        }

        public async Task<List<Group>> GetGroupsByIdsAsync(IEnumerable<int> groupIds, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            return await context.Groups
                .Where(g => groupIds.Contains(g.Id))
                .Include(g => g.Classes!)
                    .ThenInclude(c => c.Discipline)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<Group> UpdateGroupAsync(Group entity, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            context.Groups.Update(entity);
            await context.SaveChangesAsync(cancellationToken: cancellationToken);
            return entity;
        }
    }
}
