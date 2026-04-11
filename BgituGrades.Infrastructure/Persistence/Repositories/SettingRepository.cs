using BgituGrades.Data;
using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BgituGrades.Infrastructure.Persistence.Repositories
{
    
    public class SettingRepository(AppDbContext dbContext) : ISettingRepository
    {
        private readonly AppDbContext _dbContext = dbContext;
        public async Task<Setting?> GetCalendarUrlAsync(CancellationToken cancellationToken)
        {
            var url = await _dbContext.Settings
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            return url;
        }

        public async Task UpdateSettingAsync(Setting setting, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.Settings.FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (existing is null)
                await _dbContext.Settings.AddAsync(setting, cancellationToken: cancellationToken);
            else
                existing.CalendarUrl = setting.CalendarUrl;

            await _dbContext.SaveChangesAsync(cancellationToken: cancellationToken);
        }
    }
}
