using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Enums;
using BgituGrades.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BgituGrades.Infrastructure.Persistence.Repositories
{
    
    public class KeyRepository(IDbContextFactory<AppDbContext> contextFactory) : IKeyRepository
    {
        public async Task<ApiKey> CreateKeyAsync(ApiKey entity, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            await context.ApiKeys.AddAsync(entity, cancellationToken: cancellationToken);
            await context.SaveChangesAsync(cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<bool> DeleteKeyAsync(string hash, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var result = await context.ApiKeys
                .Where(k => k.LookupHash == hash)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
            return result > 0;
        }

        public async Task<ApiKey?> GetAsync(string key, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var storedKey = await context.ApiKeys.FindAsync([key], cancellationToken: cancellationToken);
            return storedKey;
        }

        public async Task<ApiKey?> GetByLookupHashAsync(string lookupHash, CancellationToken cancellationToken = default)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            var storedKey = await context.ApiKeys
                .Where(k => k.LookupHash == lookupHash)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            return storedKey;
        }

        public async Task<List<ApiKey>> GetKeysAsync(CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var keys = await context.ApiKeys
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
            return keys.OrderBy(k => Enum.Parse<Role>(k.Role!)).ToList();
        }
    }
}
