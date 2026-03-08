using Asp.Versioning;
using BgituGrades.Models.Setting;
using BgituGrades.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace BgituGrades.Controllers
{
    [Route("api/settings")]
    [ApiController]
    [Authorize(Policy = "Admin")]
    public class SettingsController(ISettingService settingService) : ControllerBase
    {
        private readonly ISettingService _settingService = settingService;

        [HttpGet]
        [ApiVersion("2.0")]
        public async Task<ActionResult<SettingResponse>> GetSettings()
        {
            var settings = await _settingService.GetSettingsAsync();
            return Ok(settings);
        }

        
        [HttpPut]
        [ApiVersion("2.0")]
        public async Task<IActionResult> UpdateSettings([FromBody] UpdateSettingRequest request)
        {
            await _settingService.UpdateSettingAsync(request);
            return NoContent();
        }
    }
}
