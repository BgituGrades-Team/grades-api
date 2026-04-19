using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BgituGrades.Infrastructure.Persistence.Repositories
{
    
    public class SettingRepository(IDbContextFactory<AppDbContext> contextFactory) : ISettingRepository
    {
        public async Task<Setting?> GetCalendarUrlAsync(CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var url = await context.Settings
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            return url;
        }

        public async Task UpdateSettingAsync(Setting setting, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var existing = await context.Settings
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (existing is null)
                await context.Settings.AddAsync(setting, cancellationToken: cancellationToken);
            else
                existing.CalendarUrl = setting.CalendarUrl;

            await context.SaveChangesAsync(cancellationToken: cancellationToken);
        }
    }
}
