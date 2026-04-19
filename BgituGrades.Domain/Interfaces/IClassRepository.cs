using BgituGrades.Domain.Entities;

namespace BgituGrades.Domain.Interfaces
{
    public interface IClassRepository
    {
        Task<List<Class>> GetClassesByDisciplineAndGroupAsync(int disciplineId, int groupId, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
        Task<Class> CreateClassAsync(Class entity, CancellationToken cancellationToken);
        Task<List<Class>> CreateClassAsync(IEnumerable<Class> entities, CancellationToken cancellationToken);
        Task<Class?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> UpdateClassAsync(Class entity, CancellationToken cancellationToken);
        Task<bool> DeleteClassAsync(int id, CancellationToken cancellationToken);
        Task DeleteAllAsync(CancellationToken cancellationToken);
        Task<List<Class>> GetAllClassesAsync(CancellationToken cancellationToken);
        Task<Dictionary<(int GroupId, int DisciplineId), List<Class>>> GetClassesByGroupIdsAndDisciplineIdsAsync(
            IEnumerable<int> groupIds, IEnumerable<int> disciplineIds, CancellationToken cancellationToken);
    }
}
