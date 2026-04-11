namespace BgituGrades.Application.DTOs
{
    public class TablePreview
    {
        public byte[]? ExcelBytes { get; set; }
        public ReportPreviewDto? Preview { get; set; }
    }
    public class ReportPreviewDto
    {
        public List<PreviewRow> Rows { get; set; } = [];
    }

    public class PreviewRow
    {
        public bool IsGroupHeader { get; set; }
        public List<string> Cells { get; set; } = [];
    }
}
