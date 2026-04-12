using BgituGrades.Domain.Entities;

namespace BgituGrades.Domain.Interfaces
{
    public interface IGroupRepository
    {
        Task<List<Group>> GetGroupsByDisciplineAsync(int disciplineId, CancellationToken cancellationToken);
        Task<List<Group>> GetAllAsync(CancellationToken cancellationToken);
        Task<Group> CreateGroupAsync(Group entity, CancellationToken cancellationToken);
        Task<List<Group>> CreateGroupAsync(IEnumerable<Group> entities, CancellationToken cancellationToken);
        Task<Group?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<Group>> GetArchivedByPeriod(int semester, int year, CancellationToken cancellationToken);
        Task<List<int>> GetCoursesAsync(CancellationToken cancellationToken);
        Task<List<Group>> GetGroupsByCoursesAsync(IEnumerable<int> courses, CancellationToken cancellationToken);
        Task<Group> UpdateGroupAsync(Group entity, CancellationToken cancellationToken);
        Task<bool> DeleteGroupAsync(int id, CancellationToken cancellationToken);
        Task DeleteAllAsync(CancellationToken cancellationToken);
        Task<List<Group>> GetGroupsByIdsAsync(IEnumerable<int> groupIds, CancellationToken cancellationToken);
        Task<List<Group>> GetByIdsAsync(IEnumerable<int> groupIds, CancellationToken cancellationToken);
        Task<List<int>> GetArchivedCoursesByPeriodAsync(int year, int semester, CancellationToken cancellationToken);
        Task<List<Group>> GetArchivedGroupsByCoursesAsync(IEnumerable<int> courses, CancellationToken cancellationToken);
        Task<List<Group>> GetArchivedGroupsByCoursesAndPeriodAsync(IEnumerable<int> courses, int year, int semester, CancellationToken cancellationToken);
    }
}
