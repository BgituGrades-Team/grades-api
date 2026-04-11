namespace BgituGrades.Domain.Entities
{
    public class ApiKey
    {
        public required string Key { get; set; }
        public string? StoredHash { get; set; }
        public string? LookupHash { get; set; }
        public required string OwnerName { get; set; }
        public string? Role { get; set; }
        public int? GroupId { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
