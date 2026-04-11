using BgituGrades.Domain.Entities;

namespace BgituGrades.Domain.Interfaces
{
    public interface IDisciplineRepository
    {
        Task<List<Discipline>> GetAllAsync(CancellationToken cancellationToken);
        Task<Discipline> CreateDisciplineAsync(Discipline entity, CancellationToken cancellationToken);
        Task<List<Discipline>> CreateDisciplineAsync(IEnumerable<Discipline> entities, CancellationToken cancellationToken);
        Task<Discipline?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<Discipline>> GetByGroupIdAsync(int groupId, CancellationToken cancellationToken);
        Task<List<Discipline>> GetByGroupIdsAsync(IEnumerable<int> groupIds, CancellationToken cancellationToken);
        Task<List<Discipline>> GetArchivedByGroupIdsAsync(IEnumerable<int> groupIds, CancellationToken cancellationToken);
        Task<bool> UpdateDisciplineAsync(Discipline entity, CancellationToken cancellationToken);
        Task<bool> DeleteDisciplineAsync(int id, CancellationToken cancellationToken);
        Task DeleteAllAsync(CancellationToken cancellationToken);
        Task<List<Discipline>> GetDisciplinesByIdsAsync(IEnumerable<int> disciplineIds, CancellationToken cancellationToken);
        Task<Dictionary<int, List<Discipline>>> GetDictByGroupIdsAsync(IEnumerable<int> groupIds, CancellationToken cancellationToken);
    }
}
