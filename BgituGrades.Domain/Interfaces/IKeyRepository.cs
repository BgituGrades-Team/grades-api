using BgituGrades.Domain.Entities;


namespace BgituGrades.Domain.Interfaces
{
    public interface IKeyRepository
    {
        Task<List<ApiKey>> GetKeysAsync(CancellationToken cancellationToken);
        Task<ApiKey> CreateKeyAsync(ApiKey entity, CancellationToken cancellationToken);
        Task<ApiKey?> GetAsync(string key, CancellationToken cancellationToken);
        Task<bool> DeleteKeyAsync(string key, CancellationToken cancellationToken);
        Task<ApiKey?> GetByLookupHashAsync(string lookupHash, CancellationToken cancellationToken = default);
    }
}
