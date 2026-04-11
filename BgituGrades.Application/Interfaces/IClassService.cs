using BgituGrades.Application.DTOs;
using BgituGrades.Application.Models.Class;
using BgituGrades.Domain.Entities;

namespace BgituGrades.Application.Interfaces
{
    public interface IClassService
    {
        Task<List<ClassDateResponse>> GetClassDatesAsync(GetClassDateRequest request, CancellationToken cancellationToken);
        Task<List<FullGradeMarkResponse>> GetMarksByWorksAsync(GetClassDateRequest request, CancellationToken cancellationToken);
        Task<List<FullGradePresenceResponse>> GetPresenceByScheduleAsync(GetClassDateRequest request, CancellationToken cancellationToken);
        Task<ClassResponse> CreateClassAsync(CreateClassRequest request, CancellationToken cancellationToken);
        Task<List<ClassResponse>> CreateClassAsync(CreateClassBulkRequest request, CancellationToken cancellationToken);
        Task<ClassResponse?> GetClassByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> DeleteClassAsync(int id, CancellationToken cancellationToken);
        Task<List<ClassDateResponse>> GenerateScheduleDatesAsync(int groupId, int disciplineId, CancellationToken cancellationToken,
            DateOnly? startDateOverride = null, DateOnly? endDateOverride = null);
        Task<List<ClassDateResponse>> GenerateScheduleDatesAsync(Group group, IEnumerable<Class> classes,
            IEnumerable<Transfer> transfers, DateOnly? startDateOverride = null, DateOnly? endDateOverride = null);
        Task<List<ClassDTO>> GetAllClassesDtoAsync(CancellationToken cancellationToken);
    }
}
