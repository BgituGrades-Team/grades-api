using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BgituGrades.Infrastructure.Persistence.Repositories
{
    public class WorkRepository(IDbContextFactory<AppDbContext> contextFactory) : IWorkRepository
    {

        public async Task<Work> CreateWorkAsync(Work entity, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            await context.Works.AddAsync(entity, cancellationToken: cancellationToken);
            await context.SaveChangesAsync(cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<bool> DeleteWorkAsync(int id, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var result = await context.Works
                .Where(w => w.Id == id)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
            return result > 0;
        }

        public async Task<Work?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var entity = await context.Works.FindAsync([id], cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<List<Work>> GetAllWorksAsync(CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var entities = await context.Works
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
            return entities;
        }

        public async Task<List<Work>> GetByDisciplineAndGroupAsync(int disciplineId, int groupId, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var entities = await context.Works
                .Where(w => w.DisciplineId == disciplineId && w.GroupId == groupId)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            return entities;
        }

        public async Task<Work> UpdateWorkAsync(Work entity, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var updatedEntity = context.Update(entity);
            await context.SaveChangesAsync(cancellationToken: cancellationToken);
            return updatedEntity.Entity;
        }

        public async Task DeleteAllAsync(CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            await context.Works.ExecuteDeleteAsync(cancellationToken: cancellationToken);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            return await context.Works.AnyAsync(w => w.Id == id, cancellationToken: cancellationToken);
        }
    }
}
