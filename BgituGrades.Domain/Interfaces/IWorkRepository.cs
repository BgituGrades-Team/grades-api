using BgituGrades.Domain.Entities;

namespace BgituGrades.Domain.Interfaces
{
    public interface IWorkRepository
    {
        Task<List<Work>> GetAllWorksAsync(CancellationToken cancellationToken);
        Task<Work> CreateWorkAsync(Work entity, CancellationToken cancellationToken);
        Task<Work?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<Work>> GetByDisciplineAndGroupAsync(int disciplineId, int groupId, CancellationToken cancellationToken);
        Task<bool> UpdateWorkAsync(Work entity, CancellationToken cancellationToken);
        Task<bool> DeleteWorkAsync(int id, CancellationToken cancellationToken);
        Task DeleteAllAsync(CancellationToken cancellationToken);
    }
}
