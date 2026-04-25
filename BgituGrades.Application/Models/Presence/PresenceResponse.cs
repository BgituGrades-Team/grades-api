using BgituGrades.Domain.Enums;

namespace BgituGrades.Application.Models.Presence
{
    public class PresenceResponse
    {
        public int Id { get; set; }
        public PresenceType IsPresent { get; set; }
        public DateOnly Date { get; set; }
        public int DisciplineId { get; set; }
        public int StudentId { get; set; }
    }

    public class GradePresenceResponse
    {
        public int ClassId { get; set; }
        public ClassType ClassType { get; set; }
        public PresenceType IsPresent { get; set; }
        public DateOnly Date { get; set; }
        public DateOnly OriginalDate { get; set; }
    }

    public class PresenceCountResponse
    {
        public int Present { get; set; }
        public int Total { get; set; }
        public string DisciplineName { get; set; } = null!;
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
    }
}
