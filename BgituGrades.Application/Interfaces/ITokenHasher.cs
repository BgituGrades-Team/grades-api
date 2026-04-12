namespace BgituGrades.Application.Interfaces
{
    public interface ITokenHasher
    {
        string Hash(string token);
        bool Verify(string token, string storedHash);
        string ComputeLookupHash(string token);
    }
}
