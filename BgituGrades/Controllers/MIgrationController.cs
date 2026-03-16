using Asp.Versioning;
using BgituGrades.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BgituGrades.Controllers
{
    [Route("api/clearDb")]
    [ApiVersion("2.0")]
    [Authorize(Policy = "Admin")]
    [ApiController]
    public class MIgrationController(IMigrationService migrationService) : ControllerBase
    {
        private readonly IMigrationService _migrationService = migrationService;

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken)
        {
            await _migrationService.DeleteAll(cancellationToken: cancellationToken);
            return NoContent();
        }
    }
}
