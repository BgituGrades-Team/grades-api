using BgituGrades.Application.DTOs;
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
    }
}
