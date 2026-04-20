using BgituGrades.Application.DTOs;
using BgituGrades.Application.Models.Class;
using BgituGrades.Domain.Entities;

namespace BgituGrades.Application.Interfaces
{
    public interface IClassService
    {
        Task<List<ClassDateResponse>> GetClassDatesAsync(int groupId, int disciplineId, CancellationToken cancellationToken);
        Task<List<FullGradeMarkResponse>> GetMarksByWorksAsync(int groupId, int disciplineId, CancellationToken cancellationToken);
        Task<List<FullGradePresenceResponse>> GetPresenceByScheduleAsync(int groupId, int disciplineId, CancellationToken cancellationToken);
        Task<ClassDTO> CreateClassAsync(ClassDTO classDto, CancellationToken cancellationToken);
        Task<List<ClassDTO>> CreateClassAsync(IEnumerable<ClassDTO> classDto, CancellationToken cancellationToken);
        Task<List<ClassDateResponse>> GenerateScheduleDatesAsync(int groupId, int disciplineId, CancellationToken cancellationToken,
            DateOnly? startDateOverride = null, DateOnly? endDateOverride = null);
        Task<List<ClassDateResponse>> GenerateScheduleDatesAsync(Group group, IEnumerable<Class> classes,
            IEnumerable<Transfer> transfers, DateOnly? startDateOverride = null, DateOnly? endDateOverride = null);
    }
}
