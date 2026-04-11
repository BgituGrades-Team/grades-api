using BgituGrades.Application.Models.Report;

namespace BgituGrades.Application.Interfaces
{
    public interface IArchivedReportService
    {
        Task<Guid> GenerateReportAsync(ArchivedReportRequest request, string connectionId, CancellationToken cancellationToken);
    }
}
