using Asp.Versioning;
using BgituGrades.Models.Report;
using BgituGrades.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BgituGrades.Controllers
{
    [Route("api/migrations")]
    [ApiVersion("2.0")]
    [ApiController]
    public class MIgrationController(IMigrationService migrationService) : ControllerBase
    {
        private readonly IMigrationService _migrationService = migrationService;

        [HttpDelete("truncate")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken)
        {
            await _migrationService.DeleteAll(cancellationToken: cancellationToken);
            return NoContent();
        }

        [HttpPost("migrate")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> MigrateAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _migrationService.ArchiveCurrentSemesterAsync(cancellationToken);
                return Ok("Архивация успешно завершена.");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("periods/all")]
        [Authorize(Policy = "Edit")]
        [ProducesResponseType(typeof(IEnumerable<PeriodResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPeriodsAsync(CancellationToken cancellationToken)
        {
            var periods = await _migrationService.GetAllPeriods(cancellationToken: cancellationToken);
            return Ok(periods);
        }
    }
}
