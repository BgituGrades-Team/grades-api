using BgituGrades.Application.DTOs;
using BgituGrades.Application.Models.Group;

namespace BgituGrades.Application.Interfaces
{
    public interface IGroupService
    {
        Task<List<GroupResponse>> GetGroupsByDisciplineAsync(int disciplineId, CancellationToken cancellationToken);
        Task<List<GroupResponse>> GetAllAsync(CancellationToken cancellationToken);
        Task<GroupResponse> CreateGroupAsync(CreateGroupRequest request, CancellationToken cancellationToken);
        Task<List<GroupResponse>> CreateGroupAsync(CreateGroupBulkRequest request, CancellationToken cancellationToken);
        Task<GroupResponse?> GetGroupByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> UpdateGroupAsync(UpdateGroupRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteGroupAsync(int id, CancellationToken cancellationToken);
        Task<List<GroupDTO>> GetAllGroupsDtoAsync(CancellationToken cancellationToken);
        Task<List<GroupDTO>> GetGroupsDtoByDisciplineAsync(int disciplineId, CancellationToken cancellationToken);
        Task<List<ArchivedGroupResponse>> GetArchivedGroupsByPeriodAsync(int semester, int year, CancellationToken cancellationToken);
        Task<List<int>> GetCoursesAsync(CancellationToken cancellationToken);
        Task<List<int>> GetArchivedCoursesByPeriodAsync(int year, int semester, CancellationToken cancellationToken);
        Task<List<GroupResponse>> GetGroupsByCoursesAsync(IEnumerable<int> courses, CancellationToken cancellationToken);
        Task<List<ArchivedGroupResponse>> GetArchivedGroupsByCoursesAndPeriodAsync(GetArchivedByCoursesRequest request, CancellationToken cancellationToken);
        Task<GroupDTO?> GetGroupDtoByIdAsync(int id, CancellationToken cancellationToken);
    }
}
