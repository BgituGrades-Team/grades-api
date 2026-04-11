using BgituGrades.Domain.Entities;

namespace BgituGrades.Domain.Interfaces
{
    public interface ISettingRepository
    {
        Task<Setting?> GetCalendarUrlAsync(CancellationToken cancellationToken);
        Task UpdateSettingAsync(Setting setting, CancellationToken cancellationToken);
    }
}
