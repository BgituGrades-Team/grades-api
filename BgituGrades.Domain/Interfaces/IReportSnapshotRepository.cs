using BgituGrades.Domain.Entities;

namespace BgituGrades.Domain.Interfaces
{
    public interface IReportSnapshotRepository
    {
        Task<List<ReportSnapshot>> GetAllReportSnapshotsAsync(CancellationToken cancellationToken);
        Task<ReportSnapshot?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> DeleteReportSnapshotAsync(int id, CancellationToken cancellationToken);
        Task<List<ReportSnapshot>> GetReportSnapshotsByGroupAndDisciplineAsync(int groupId, int disciplineId, CancellationToken cancellationToken);
        Task<List<ReportSnapshot>> GetReportSnapshotsByYearAndSemesterAsync(
            int year, int semester, CancellationToken cancellationToken);
        Task<Dictionary<(int GroupId, int DisciplineId), List<ReportSnapshot>>> GetReportSnapshotsByGroupIdsAsync(
            IEnumerable<int> groupIds, CancellationToken cancellationToken);
    }
}
