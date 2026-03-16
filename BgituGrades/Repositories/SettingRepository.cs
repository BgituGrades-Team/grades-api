using BgituGrades.Data;
using BgituGrades.Entities;
using Microsoft.EntityFrameworkCore;

namespace BgituGrades.Repositories
{
    public interface ISettingRepository
    {
        Task<Setting?> GetCalendarUrlAsync(CancellationToken cancellationToken);
        Task UpdateSettingAsync(Setting setting, CancellationToken cancellationToken);
    }
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
                _dbContext.Settings.Add(setting);
            else
            {
                existing.CalendarUrl = setting.CalendarUrl;
            }

            await _dbContext.SaveChangesAsync(cancellationToken: cancellationToken);
        }
    }
}
