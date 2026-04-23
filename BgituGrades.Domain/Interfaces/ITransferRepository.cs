using BgituGrades.Domain.Entities;

namespace BgituGrades.Domain.Interfaces
{
    public interface ITransferRepository
    {
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
        Task<bool> OriginalDateExistsAsync(DateOnly date, CancellationToken cancellationToken);
        Task<Transfer> CreateTransferAsync(Transfer entity, CancellationToken cancellationToken);
        Task<Transfer?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<Transfer?> GetByClassIdAndDateAsync(int classId, DateOnly originalDate, CancellationToken cancellationToken);
        Task<bool> UpdateTransferAsync(Transfer entity, CancellationToken cancellationToken);
        Task<bool> DeleteTransferAsync(int id, CancellationToken cancellationToken);
        Task DeleteAllAsync(CancellationToken cancellationToken);
        Task<List<Transfer>> GetTransfersByGroupAndDisciplineAsync(int groupId, int disciplineId, CancellationToken cancellationToken);
        Task<Dictionary<(int GroupId, int DisciplineId), List<Transfer>>> GetTransfersByGroupIdsAsync(
            IEnumerable<int> groupIds, CancellationToken cancellationToken);
    }
}
