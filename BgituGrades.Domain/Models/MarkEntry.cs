namespace BgituGrades.Domain.Models
{
    public class MarkEntry
    {
        public int WorkId { get; set; }
        public required string Name { get; set; }
        public string? Value { get; set; }
    }
}
