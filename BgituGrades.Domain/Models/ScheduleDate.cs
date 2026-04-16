using BgituGrades.Domain.Enums;

namespace BgituGrades.Domain.Models
{
    public class ScheduleDate
    {
        public int Id { get; set; }
        public DateOnly OriginalDate { get; set; }
        public DateOnly Date { get; set; }
        public ClassType ClassType { get; set; }
        public DateTime StartTime { get; set; }
    }
}
