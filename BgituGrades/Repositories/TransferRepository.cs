using BgituGrades.Data;
using BgituGrades.Entities;
using Microsoft.EntityFrameworkCore;

namespace BgituGrades.Repositories
{
    public interface ITransferRepository
    {
        Task<IEnumerable<Transfer>> GetAllTransfersAsync(CancellationToken cancellationToken);
        Task<Transfer> CreateTransferAsync(Transfer entity, CancellationToken cancellationToken);
        Task<Transfer?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> UpdateTransferAsync(Transfer entity, CancellationToken cancellationToken);
        Task<bool> DeleteTransferAsync(int id, CancellationToken cancellationToken);
        Task DeleteAllAsync(CancellationToken cancellationToken);
        Task<IEnumerable<Transfer>> GetTransfersByGroupAndDisciplineAsync(int groupId, int disciplineId, CancellationToken cancellationToken);
    }

    public class TransferRepository(AppDbContext dbContext) : ITransferRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<Transfer> CreateTransferAsync(Transfer entity, CancellationToken cancellationToken)
        {
            await _dbContext.Transfers.AddAsync(entity, cancellationToken: cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<bool> DeleteTransferAsync(int id, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Transfers
                .Where(t => t.Id == id)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
            return result > 0;
        }

        public async Task<Transfer?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Transfers.FindAsync([id], cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<IEnumerable<Transfer>> GetAllTransfersAsync(CancellationToken cancellationToken)
        {
            var entities = await _dbContext.Transfers
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
            return entities;
        }

        public async Task<bool> UpdateTransferAsync(Transfer entity, CancellationToken cancellationToken)
        {
            _dbContext.Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken: cancellationToken);
            return true;
        }

        public async Task DeleteAllAsync(CancellationToken cancellationToken)
        {
            await _dbContext.Transfers.ExecuteDeleteAsync(cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<Transfer>> GetTransfersByGroupAndDisciplineAsync(int groupId, int disciplineId, CancellationToken cancellationToken)
        {
            var entities =  await _dbContext.Transfers
                .Where(t => t.DisciplineId == disciplineId &&
                            t.GroupId == groupId)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
            return entities;
        }
    }

}
