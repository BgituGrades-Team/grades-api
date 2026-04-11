namespace BgituGrades.Domain.Models
{
    public class StudentMarkResult
    {
        public int StudentId { get; set; }
        public string? Name { get; set; }
        public List<MarkEntry>? Marks { get; set; }
    }
}
