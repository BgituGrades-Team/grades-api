using BgituGrades.Application.DTOs;

namespace BgituGrades.Application.Interfaces
{
    public interface IGroupService
    {
        Task<List<GroupDTO>> GetGroupsByDisciplineAsync(int disciplineId, CancellationToken cancellationToken);
        Task<List<GroupDTO>> GetAllAsync(CancellationToken cancellationToken);
        Task<GroupDTO> CreateGroupAsync(GroupDTO request, CancellationToken cancellationToken);
        Task<List<GroupDTO>> CreateGroupAsync(IEnumerable<GroupDTO> request, CancellationToken cancellationToken);
        Task<GroupDTO?> GetGroupByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> DeleteGroupAsync(int id, CancellationToken cancellationToken);
        Task<List<GroupDTO>> GetArchivedGroupsByPeriodAsync(int semester, int year, CancellationToken cancellationToken);
        Task<List<int>> GetCoursesAsync(CancellationToken cancellationToken);
        Task<List<int>> GetArchivedCoursesByPeriodAsync(int year, int semester, CancellationToken cancellationToken);
        Task<List<GroupDTO>> GetGroupsByCoursesAsync(IEnumerable<int> courses, CancellationToken cancellationToken);
    }
}
