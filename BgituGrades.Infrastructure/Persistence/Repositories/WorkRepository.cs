using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BgituGrades.Infrastructure.Persistence.Repositories
{
    public class WorkRepository(AppDbContext dbContext) : IWorkRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<Work> CreateWorkAsync(Work entity, CancellationToken cancellationToken)
        {
            await _dbContext.Works.AddAsync(entity, cancellationToken: cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<bool> DeleteWorkAsync(int id, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Works
                .Where(w => w.Id == id)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
            return result > 0;
        }

        public async Task<Work?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Works.FindAsync([id], cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<List<Work>> GetAllWorksAsync(CancellationToken cancellationToken)
        {
            var entities = await _dbContext.Works
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
            return entities;
        }

        public async Task<List<Work>> GetByDisciplineAndGroupAsync(int disciplineId, int groupId, CancellationToken cancellationToken)
        {
            var entities = await _dbContext.Works
                .Where(w => w.DisciplineId == disciplineId && w.GroupId == groupId)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            return entities;
        }

        public async Task<Work> UpdateWorkAsync(Work entity, CancellationToken cancellationToken)
        {
            var updatedEntity = _dbContext.Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken: cancellationToken);
            return updatedEntity.Entity;
        }

        public async Task DeleteAllAsync(CancellationToken cancellationToken)
        {
            await _dbContext.Works.ExecuteDeleteAsync(cancellationToken: cancellationToken);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken) => 
            await _dbContext.Works.AnyAsync(w => w.Id == id, cancellationToken: cancellationToken);
    }
}
