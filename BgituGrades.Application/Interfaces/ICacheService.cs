using Microsoft.Extensions.Caching.Hybrid;

namespace BgituGrades.Application.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetOrCreateAsync<T>(
            string key,
            Func<CancellationToken, ValueTask<T>> factory,
            IEnumerable<string>? tags = null,
            HybridCacheEntryOptions? options = null,
            CancellationToken ct = default);
        Task RemoveAsync(string key, CancellationToken ct = default);
        Task RemoveManyAsync(IEnumerable<string> keys, CancellationToken ct = default);
        Task RemoveByTagAsync(string tag, CancellationToken ct = default);
        Task RemoveAllAsync(CancellationToken ct = default);
    }
}
