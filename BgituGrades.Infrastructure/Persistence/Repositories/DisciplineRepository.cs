using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Interfaces;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace BgituGrades.Infrastructure.Persistence.Repositories
{
    

    public class DisciplineRepository(IDbContextFactory<AppDbContext> contextFactory) : IDisciplineRepository
    {
        public async Task<Discipline> CreateDisciplineAsync(Discipline entity, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            await context.Disciplines.AddAsync(entity, cancellationToken: cancellationToken);
            await context.SaveChangesAsync(cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<List<Discipline>> CreateDisciplineAsync(IEnumerable<Discipline> entities, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var entityList = entities.ToList();
            var bulkConfig = new BulkConfig { SetOutputIdentity = true };
            await context.BulkInsertAsync(entityList, bulkConfig, cancellationToken: cancellationToken);
            return entityList;
        }

        public async Task DeleteAllAsync(CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            await context.Disciplines.ExecuteDeleteAsync(cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteDisciplineAsync(int id, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var result = await context.Disciplines
                .Where(d => d.Id == id)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
            return result > 0;
        }

        public async Task<List<Discipline>> GetAllAsync(CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            return await context.Disciplines
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<List<Discipline>> GetByGroupIdAsync(int groupId, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            return await context.Disciplines
                .Where(d => d.Classes!.Any(c => c.GroupId == groupId))
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<List<Discipline>> GetByGroupIdsAsync(IEnumerable<int> groupIds, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            return await context.Disciplines
                .Where(d => d.Classes!.Any(c => groupIds.Contains(c.GroupId)))
                    .Include(d => d.Classes!.Where(c => groupIds.Contains(c.GroupId)))
                .AsNoTracking()
                .Distinct()
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<Discipline?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            return await context.Disciplines.FindAsync([id], cancellationToken: cancellationToken);
        }

        public async Task<List<Discipline>> GetDisciplinesByIdsAsync(IEnumerable<int> disciplineIds, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            return await context.Disciplines
                .AsNoTracking()
                .Where(d => disciplineIds.Contains(d.Id))
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<bool> UpdateDisciplineAsync(Discipline entity, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            context.Disciplines.Update(entity);
            await context.SaveChangesAsync(cancellationToken: cancellationToken);
            return true;
        }

        public async Task<Dictionary<int, List<Discipline>>> GetDictByGroupIdsAsync(
            IEnumerable<int> groupIds, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);

            var classes = await context.Classes
                .Where(c => groupIds.Contains(c.GroupId))
                .Select(c => new { c.GroupId, c.DisciplineId })
                .AsNoTracking()
                .Distinct()
                .ToListAsync(cancellationToken);

            var disciplineIds = classes.Select(c => c.DisciplineId).Distinct().ToList();

            var disciplines = await context.Disciplines
                .Where(d => disciplineIds.Contains(d.Id))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var disciplineById = disciplines.ToDictionary(d => d.Id);

            return classes
                .GroupBy(c => c.GroupId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(c => disciplineById[c.DisciplineId])
                          .DistinctBy(d => d.Id)
                          .ToList());
        }

        public async Task<List<Discipline>> GetArchivedByGroupIdsAsync(IEnumerable<int> groupIds, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var archivedDisciplines = await context.ReportSnapshots
                .AsNoTracking()
                .Where(r => groupIds.Contains(r.GroupId))
                .Select(r => new
                {
                    r.DisciplineId,
                    r.DisciplineName
                })
                .Distinct()
                .Select(r => new Discipline { Id = r.DisciplineId, Name = r.DisciplineName })
                .ToListAsync(cancellationToken: cancellationToken);
            return archivedDisciplines;
        }
    }
}
