using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Interfaces;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;


namespace BgituGrades.Infrastructure.Persistence.Repositories
{
    public class ClassRepository(IDbContextFactory<AppDbContext> contextFactory) : IClassRepository
    {
        public async Task<Class> CreateClassAsync(Class entity, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            await context.Classes.AddAsync(entity, cancellationToken: cancellationToken);
            await context.SaveChangesAsync(cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<List<Class>> CreateClassAsync(IEnumerable<Class> entities, CancellationToken cancellationToken)
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
            await context.Classes.ExecuteDeleteAsync(cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteClassAsync(int id, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var result = await context.Classes
                .Where(c => c.Id == id)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
            return result > 0;
        }

        public async Task<Class?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var entity = await context.Classes.FindAsync([id], cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<List<Class>> GetClassesByDisciplineAndGroupAsync(int disciplineId, int groupId, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var entities = await context.Classes
                .Where(c => c.GroupId == groupId && c.DisciplineId == disciplineId)
                .OrderBy(c => c.Weeknumber)
                    .ThenBy(c => c.WeekDay)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
            return entities;
        }

        public async Task<bool> UpdateClassAsync(Class entity, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            context.Update(entity);
            return await context.SaveChangesAsync(cancellationToken: cancellationToken) > 0;
        }

        public async Task<List<Class>> GetAllClassesAsync(CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            return await context.Classes
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<Dictionary<(int GroupId, int DisciplineId), List<Class>>> GetClassesByGroupIdsAndDisciplineIdsAsync(
            IEnumerable<int> groupIds, IEnumerable<int> disciplineIds, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var classes = await context.Classes
                .Where(c => groupIds.Contains(c.GroupId) && disciplineIds.Contains(c.DisciplineId))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return classes
                .GroupBy(c => (c.GroupId, c.DisciplineId))
                .ToDictionary(
                    g => g.Key,
                    g => g.ToList());
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            return await context.Classes.AnyAsync(c => c.Id == id, cancellationToken: cancellationToken);
        }
    }
}
