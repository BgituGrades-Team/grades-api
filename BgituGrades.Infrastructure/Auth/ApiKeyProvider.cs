using AspNetCore.Authentication.ApiKey;
using BgituGrades.Application.Caching;
using BgituGrades.Application.Interfaces;
using BgituGrades.Domain.Interfaces;
using Microsoft.Extensions.Caching.Hybrid;

namespace BgituGrades.Infrastructure.Auth
{
    public class ApiKeyProvider(ITokenHasher hasher, IKeyRepository keyRepository, ICacheService cacheService) : IApiKeyProvider
    {
        private readonly ITokenHasher _hasher = hasher;
        private readonly IKeyRepository _keyRepository = keyRepository;
        private readonly ICacheService _cacheService = cacheService;

        private static readonly HybridCacheEntryOptions DefaultOptions = new()
        {
            Expiration = TimeSpan.FromMinutes(10),
            LocalCacheExpiration = TimeSpan.FromMinutes(5)
        };

        public async Task<IApiKey?> ProvideAsync(string key)
        {
            var lookupHash = _hasher.ComputeLookupHash(key);

            var storedKey = await _cacheService.GetOrCreateAsync(
                key: CacheKeys.KeyVerified(lookupHash),
                factory: async token =>
                {
                    var k = await _keyRepository.GetByLookupHashAsync(lookupHash, token);
                    if (k is null) return null;
                    if (!_hasher.Verify(key, k.StoredHash!)) return null;
                    if (k.ExpiryDate is not null && k.ExpiryDate < DateTime.UtcNow) return null;
                    return k;
                },
                options: DefaultOptions);

            return storedKey is null ? null : new ApiKeyAuthModel(storedKey);
        }
    }
}
