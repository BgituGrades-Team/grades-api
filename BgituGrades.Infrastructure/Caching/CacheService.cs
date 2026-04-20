using BgituGrades.Application.Caching;
using BgituGrades.Application.Interfaces;
using Microsoft.Extensions.Caching.Hybrid;

namespace BgituGrades.Infrastructure.Caching
{
    public sealed class CacheService(HybridCache cache) : ICacheService
    {
        public async Task<T> GetOrCreateAsync<T>(
            string key,
            Func<CancellationToken, ValueTask<T>> factory,
            IEnumerable<string>? tags = null,
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

        public async Task RemoveByTagAsync(string tag, CancellationToken ct = default)
        {
            await cache.RemoveByTagAsync(tag, ct);
        }

        public async Task RemoveAllAsync(CancellationToken ct = default)
        {
            await cache.RemoveByTagAsync(CacheTags.All(), ct);
        }
    }
}
