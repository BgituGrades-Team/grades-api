using BgituGrades.Application.Models.Report;

namespace BgituGrades.Application.Interfaces
{
    public interface IReportService
    {
        Task<Guid> GenerateReportAsync(ReportRequest request, string connectionId, CancellationToken cancellationToken);
    }
}
