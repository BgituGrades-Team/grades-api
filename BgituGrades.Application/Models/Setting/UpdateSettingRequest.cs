using System.ComponentModel.DataAnnotations;

namespace BgituGrades.Application.Models.Setting
{
    public class UpdateSettingRequest
    {
        [Required]
        public string? CalendarUrl { get; set; }
    }
}
