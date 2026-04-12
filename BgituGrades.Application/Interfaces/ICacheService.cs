using Microsoft.Extensions.Caching.Hybrid;

namespace BgituGrades.Application.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetOrCreateAsync<T>(
            string key,
            Func<CancellationToken, ValueTask<T>> factory,
            HybridCacheEntryOptions? options = null,
            CancellationToken ct = default);
        Task RemoveAsync(string key, CancellationToken ct = default);
        Task RemoveManyAsync(IEnumerable<string> keys, CancellationToken ct = default);
    }
}
