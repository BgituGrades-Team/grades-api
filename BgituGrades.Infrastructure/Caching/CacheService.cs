using BgituGrades.Application.Interfaces;
using Microsoft.Extensions.Caching.Hybrid;

namespace BgituGrades.Infrastructure.Caching
{
    public sealed class CacheService(HybridCache cache) : ICacheService
    {
        public async Task<T> GetOrCreateAsync<T>(
            string key, 
            Func<CancellationToken, ValueTask<T>> factory,
            HybridCacheEntryOptions? options = null,
            CancellationToken ct = default)
        {
            return await cache.GetOrCreateAsync(key, factory, options, cancellationToken: ct);
        }

        public async Task RemoveAsync(string key, CancellationToken ct = default)
        {
            await cache.RemoveAsync(key, ct);
        }

        public async Task RemoveManyAsync(IEnumerable<string> keys, CancellationToken ct = default)
        {
            await cache.RemoveAsync(keys, ct);
        }
    }
}
