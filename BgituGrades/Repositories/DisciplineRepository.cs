using BgituGrades.Data;
using BgituGrades.Entities;
using Microsoft.EntityFrameworkCore;

namespace BgituGrades.Repositories
{
    public interface IDisciplineRepository {
        Task<IEnumerable<Discipline>> GetAllAsync(CancellationToken cancellationToken);
        Task<Discipline> CreateDisciplineAsync(Discipline entity, CancellationToken cancellationToken);
        Task<Discipline?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<Discipline?>> GetByGroupIdAsync(int groupId, CancellationToken cancellationToken);
        Task<IEnumerable<Discipline?>> GetByGroupIdsAsync(int[] groupIds, CancellationToken cancellationToken);
        Task<bool> UpdateDisciplineAsync(Discipline entity, CancellationToken cancellationToken);
        Task<bool> DeleteDisciplineAsync(int id, CancellationToken cancellationToken);
        Task DeleteAllAsync(CancellationToken cancellationToken);
        Task<IEnumerable<Discipline>> GetDisciplinesByIdsAsync(int[] disciplineIds, CancellationToken cancellationToken);
    }

    public class DisciplineRepository(IDbContextFactory<AppDbContext> contextFactory) : IDisciplineRepository
    {
        public async Task<Discipline> CreateDisciplineAsync(Discipline entity, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            await context.Disciplines.AddAsync(entity);
            await context.SaveChangesAsync(cancellationToken: cancellationToken);  
            return entity;
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

        public async Task<IEnumerable<Discipline>> GetAllAsync(CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            return await context.Disciplines.AsNoTracking().ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<Discipline?>> GetByGroupIdAsync(int groupId, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            return await context.Disciplines
                .Where(d => d.Classes!.Any(c => c.GroupId == groupId))
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<Discipline?>> GetByGroupIdsAsync(int[] groupIds, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            return await context.Disciplines
                .Where(d => d.Classes.Any(c => groupIds.Contains(c.GroupId)))
                .Distinct()
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<Discipline?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            return await context.Disciplines.FindAsync([id], cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<Discipline>> GetDisciplinesByIdsAsync(int[] disciplineIds, CancellationToken cancellationToken)
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
    }
}
