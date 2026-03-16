using BgituGrades.Data;
using BgituGrades.Entities;
using Microsoft.EntityFrameworkCore;

namespace BgituGrades.Repositories
{
    public interface IKeyRepository
    {
        Task<IEnumerable<ApiKey>> GetKeysAsync(CancellationToken cancellationToken);
        Task<ApiKey> CreateKeyAsync(ApiKey entity, CancellationToken cancellationToken);
        Task<ApiKey?> GetAsync(string key, CancellationToken cancellationToken);
        Task<bool> DeleteKeyAsync(string key, CancellationToken cancellationToken);
        Task<ApiKey?> GetByLookupHashAsync(string lookupHash);
    }
    public class KeyRepository(AppDbContext dbContext) : IKeyRepository
    {
        private readonly AppDbContext _dbContext = dbContext;
        public async Task<ApiKey> CreateKeyAsync(ApiKey entity, CancellationToken cancellationToken)
        {
            await _dbContext.ApiKeys.AddAsync(entity, cancellationToken: cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<bool> DeleteKeyAsync(string key, CancellationToken cancellationToken)
        {
            var result = await _dbContext.ApiKeys
                .Where(k => k.Key == key)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
            return result > 0;
        }

        public async Task<ApiKey?> GetAsync(string key, CancellationToken cancellationToken)
        {
            var storedKey = await _dbContext.ApiKeys.FindAsync([key], cancellationToken: cancellationToken);
            return storedKey;
        }

        public async Task<ApiKey?> GetByLookupHashAsync(string lookupHash)
        {
            var storedKey = await _dbContext.ApiKeys.Where(k => k.LookupHash == lookupHash).FirstOrDefaultAsync();
            return storedKey;
        }

        public async Task<IEnumerable<ApiKey>> GetKeysAsync(CancellationToken cancellationToken)
        {
            var keys = await _dbContext.ApiKeys.AsNoTracking().ToListAsync(cancellationToken: cancellationToken);
            return keys;
        }
    }
}
