using BgituGrades.Application.Models.Setting;

namespace BgituGrades.Application.Interfaces
{
    public interface ISettingService
    {
        public Task<SettingResponse> GetSettingsAsync(CancellationToken cancellationToken);
        public Task UpdateSettingAsync(UpdateSettingRequest request, CancellationToken cancellationToken);

    }
}
