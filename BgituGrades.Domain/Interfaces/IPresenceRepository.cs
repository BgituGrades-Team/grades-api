using BgituGrades.Domain.Entities;


namespace BgituGrades.Domain.Interfaces
{
    public interface IPresenceRepository
    {
        Task<List<Presence>> GetAllPresencesAsync(CancellationToken cancellationToken);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
        Task<Presence> CreatePresenceAsync(Presence entity, CancellationToken cancellationToken);
        Task<List<Presence>> GetPresencesByDisciplineAndGroupAsync(int disciplineId, int groupId, CancellationToken cancellationToken);
        Task<List<Presence>> GetPresencesByDisciplinesAndGroupsAsync(IEnumerable<int> disciplineIds, IEnumerable<int> groupIds, CancellationToken cancellationToken);
        Task<Presence?> GetAsync(Presence entity, CancellationToken cancellationToken);
        Task<bool> DeletePresenceByStudentAndDateAsync(int studentId, DateOnly date, CancellationToken cancellationToken);
        Task UpdatePresenceAsync(Presence entity, CancellationToken cancellationToken);
        Task DeleteAllAsync(CancellationToken cancellationToken);
        Task<Presence?> GetPresenceByIdAsync(int id, CancellationToken cancellationToken);
        Task<(int present, int total)?> GetPresenceCountAsync(
            string groupName, string disciplineName,
            DateOnly date, TimeOnly startTime,
            CancellationToken cancellationToken);
        Task<(int present, int total, string GroupKey)?> GetPresenceCountByClassAsync(int classId, DateOnly date, CancellationToken cancellationToken);
    }
}
