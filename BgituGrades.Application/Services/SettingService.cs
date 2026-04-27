using AutoMapper;
using BgituGrades.Application.Interfaces;
using BgituGrades.Application.Models.Setting;
using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Interfaces;

namespace BgituGrades.Application.Services
{

    public class SettingService(ISettingRepository settingRepository, IMapper mapper) : ISettingService
    {
        private readonly ISettingRepository _settingRepository = settingRepository;
        private readonly IMapper _mapper = mapper;
        public async Task<SettingResponse> GetSettingsAsync(CancellationToken cancellationToken)
        {
            var calendarUrl = await _settingRepository.GetCalendarUrlAsync(cancellationToken: cancellationToken);
            var result = _mapper.Map<SettingResponse>(calendarUrl);
            return result;
        }

        public async Task UpdateSettingAsync(UpdateSettingRequest request, CancellationToken cancellationToken)
        {
            var setting = _mapper.Map<Setting>(request);
            await _settingRepository.UpdateSettingAsync(setting, cancellationToken: cancellationToken);
        }
    }
}
