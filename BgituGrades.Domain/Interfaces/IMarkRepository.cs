using BgituGrades.Domain.Entities;

namespace BgituGrades.Domain.Interfaces
{
    public interface IMarkRepository
    {
        Task<List<Mark>> GetAllMarksAsync(CancellationToken cancellationToken);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
        Task<Mark> CreateMarkAsync(Mark entity, CancellationToken cancellationToken);
        Task<List<Mark>> GetMarksByDisciplineAndGroupAsync(int disciplineId, int groupId, CancellationToken cancellationToken);
        Task<bool> UpdateMarkAsync(Mark entity, CancellationToken cancellationToken);
        Task<bool> DeleteMarkByStudentAndWorkAsync(int studentId, int workId, CancellationToken cancellationToken);
        Task<Mark?> GetMarkByStudentAndWorkAsync(int studentId, int workId, CancellationToken cancellationToken);
        Task DeleteAllAsync(CancellationToken cancellationToken);
        Task<double> GetAverageMarkByStudentAndDisciplineAsync(int studentId, int disciplineId, CancellationToken cancellationToken);
        Task<List<Mark>> GetMarksByDisciplinesAndGroupsAsync(IEnumerable<int> disciplinesIds, IEnumerable<int> groupsIds, CancellationToken cancellationToken);
        Task<Mark?> GetMarkByIdAsync(int id, CancellationToken cancellationToken);
    }
}
