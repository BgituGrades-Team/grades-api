using BgituGrades.Application.Models.Class;
using BgituGrades.Application.Models.Presence;

namespace BgituGrades.Application.Interfaces
{
    public interface IPresenceService
    {
        Task<List<PresenceResponse>> GetAllPresencesAsync(CancellationToken cancellationToken);
        Task<PresenceResponse> CreatePresenceAsync(CreatePresenceRequest request, CancellationToken cancellationToken);
        Task<List<PresenceResponse>> GetPresencesByDisciplineAndGroupAsync(GetPresenceByDisciplineAndGroupRequest request, CancellationToken cancellationToken);
        Task<bool> DeletePresenceByStudentAndDateAsync(DeletePresenceByStudentAndDateRequest request, CancellationToken cancellationToken);
        Task UpdatePresenceAsync(UpdatePresenceRequest request, CancellationToken cancellationToken);
        Task<FullGradePresenceResponse> UpdateOrCreatePresenceAsync(UpdatePresenceGradeRequest request, CancellationToken cancellationToken);
        Task<(int present, int total, TimeOnly StartTime, string DisciplineName, DateOnly Date)?> GetPresenceCountAsync(
            string groupName, string disciplineName,
            DateOnly date, TimeOnly startTime,
            CancellationToken cancellationToken);
        Task<(int present, int total, string GroupKey, TimeOnly StartTime, string DisciplineName, DateOnly Date)?> GetPresenceCountByClassAsync(
            int classId, DateOnly date,
            CancellationToken cancellationToken);
    }
}
