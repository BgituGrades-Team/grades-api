using AspNetCore.Authentication.ApiKey;
using BgituGrades.Domain.Interfaces;
using BgituGrades.Infrastructure.Security;

namespace BgituGrades.Infrastructure.Auth
{
    public class ApiKeyProvider(ITokenHasher hasher, IKeyRepository keyRepository) : IApiKeyProvider
    {
        private readonly ITokenHasher _hasher = hasher;
        private readonly IKeyRepository _keyRepository = keyRepository;

        public async Task<IApiKey?> ProvideAsync(string key)
        {
            var lookupHash = _hasher.ComputeLookupHash(key);
            var storedKey = await _keyRepository.GetByLookupHashAsync(lookupHash);

            if (storedKey is null) return null;

            var verified = _hasher.Verify(key, storedKey.StoredHash!);
            if (!verified) return null;

            if (storedKey.ExpiryDate is not null && storedKey.ExpiryDate < DateTime.UtcNow) return null;

            return storedKey;
        }
    }
}
