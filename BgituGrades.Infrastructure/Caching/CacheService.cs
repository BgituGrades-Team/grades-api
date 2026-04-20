using BgituGrades.Application.Interfaces;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BgituGrades.Infrastructure.Caching
{
    public sealed class CacheService(HybridCache cache, ILogger<CacheService> _logger) : ICacheService
    {
        public async Task<T> GetOrCreateAsync<T>(
            string key, Func<CancellationToken, ValueTask<T>> factory,
            HybridCacheEntryOptions? options = null, CancellationToken ct = default)
        {
            var sw = Stopwatch.StartNew();
            var result = await cache.GetOrCreateAsync(key, factory, options, cancellationToken: ct);
            _logger.LogInformation("Cache key: {key}, Time: {ms}ms", key, sw.ElapsedMilliseconds);
            return result;
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
