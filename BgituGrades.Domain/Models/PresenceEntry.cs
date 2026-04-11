using BgituGrades.Domain.Enums;

namespace BgituGrades.Domain.Models
{
    public class PresenceEntry
    {
        public int ClassId { get; set; }
        public ClassType ClassType { get; set; }
        public PresenceType IsPresent { get; set; }
        public DateOnly Date { get; set; }
    }
}
