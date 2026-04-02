using BgituGrades.DTO;

namespace BgituGrades.Models.Report
{
    public class ProgressReportResponse
    {
        public required string ReportId {  get; set; }
        public int Progress {  get; set; }
        public required string Description {  get; set; }
    }

    public class ReadyReportResponse
    {
        public required string ReportId { get; set; }
        public required string Link { get; set; }
        public ReportPreviewDto? Preview { get; set; }
    }
}
