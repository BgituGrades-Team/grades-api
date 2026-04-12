namespace BgituGrades.Application.DTOs
{
    public class KeyDTO
    {
        public string? Key { get; set; }
        public string? StoredHash { get; set; }
        public string? LookupHash { get; set; }
        public string? OwnerName { get; set; }
        public string? Role { get; set; }
        public int? GroupId { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
