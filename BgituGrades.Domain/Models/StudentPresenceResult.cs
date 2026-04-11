namespace BgituGrades.Domain.Models
{
    public class StudentPresenceResult
    {
        public int StudentId { get; set; }
        public string? Name { get; set; }
        public List<PresenceEntry>? Presences { get; set; }
    }
}
