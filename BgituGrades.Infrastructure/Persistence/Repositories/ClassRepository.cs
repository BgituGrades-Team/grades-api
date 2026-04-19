using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Interfaces;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;


namespace BgituGrades.Infrastructure.Persistence.Repositories
{ 
    public class ClassRepository(AppDbContext dbContext, IDbContextFactory<AppDbContext> contextFactory) : IClassRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<Class> CreateClassAsync(Class entity, CancellationToken cancellationToken)
        {
            await _dbContext.Classes.AddAsync(entity, cancellationToken: cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<List<Class>> CreateClassAsync(IEnumerable<Class> entities, CancellationToken cancellationToken)
        {
            var entityList = entities.ToList();
            var bulkConfig = new BulkConfig { SetOutputIdentity = true };
            await _dbContext.BulkInsertAsync(entityList, bulkConfig, cancellationToken: cancellationToken);
            return entityList;
        }

        public async Task DeleteAllAsync(CancellationToken cancellationToken)
        {
            await _dbContext.Classes.ExecuteDeleteAsync(cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteClassAsync(int id, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Classes
                .Where(c => c.Id == id)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
            return result > 0;
        }

        public async Task<Class?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Classes.FindAsync([id], cancellationToken: cancellationToken);
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
            _dbContext.Update(entity);
            return await _dbContext.SaveChangesAsync(cancellationToken: cancellationToken) > 0;
        }

        public async Task<List<Class>> GetAllClassesAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Classes
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<Dictionary<(int GroupId, int DisciplineId), List<Class>>> GetClassesByGroupIdsAndDisciplineIdsAsync(
            IEnumerable<int> groupIds, IEnumerable<int> disciplineIds, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var classes = await context.Classes
                .Where(c => groupIds.Contains(c.GroupId) && disciplineIds.Contains(c.DisciplineId))
                .ToListAsync(cancellationToken);

            return classes
                .GroupBy(c => (c.GroupId, c.DisciplineId))
                .ToDictionary(
                    g => g.Key,
                    g => g.ToList());
        }

        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
