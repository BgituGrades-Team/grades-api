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
                key: CacheKeys.KeyByLookUpHash(lookupHash),
                factory: async token => await _keyRepository.GetByLookupHashAsync(lookupHash),
                options: DefaultOptions);

            if (storedKey is null) return null;

            var verified = _hasher.Verify(key, storedKey.StoredHash!);
            if (!verified) return null;

            if (storedKey.ExpiryDate is not null && storedKey.ExpiryDate < DateTime.UtcNow) return null;

            return new ApiKeyAuthModel(storedKey); ;
        }
    }
}
