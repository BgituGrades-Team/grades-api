using BgituGrades.Entities;
using System.ComponentModel.DataAnnotations;

namespace BgituGrades.Models.Key
{
    public class DeleteKeyRequest
    {
        [Required]
        public required string DeleteKey { get; set; }
    }

    public class CreateKeyRequest
    {
        [Required]
        public Role Role { get; set; }
        public int? GroupId { get; set; } = null;
    }

    public class CreateSharedKeyRequest
    {
        [Required]
        public int GroupId { get; set; }
    }
}
