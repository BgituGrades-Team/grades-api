using Asp.Versioning;
using BgituGrades.Application.Interfaces;
using BgituGrades.Application.Models.Discipline;
using BgituGrades.Application.Models.Student;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BgituGrades.Controllers
{
    [Route("api/discipline")]
    [ApiController]
    public class DisciplineController(IDisciplineService DisciplineService) : ControllerBase
    {
        private readonly IDisciplineService _disciplineService = DisciplineService;

        [HttpGet("all")]
        [ApiVersion("2.0")]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(List<DisciplineResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<DisciplineResponse>>> GetDisciplines(CancellationToken cancellationToken)
        {
            var Disciplines = await _disciplineService.GetAllDisciplinesAsync(cancellationToken: cancellationToken);
            return Ok(Disciplines);
        }

        [HttpGet]
        [ApiVersion("2.0")]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(List<DisciplineResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<DisciplineResponse>>> GetDisciplinesByGroupIds([FromQuery] GetDisciplineByGroupIdsRequest request, CancellationToken cancellationToken)
        {
            var disciplines = await _disciplineService.GetDisciplineByGroupIdAsync(request.GroupIds.Values, cancellationToken: cancellationToken);
            return Ok(disciplines);
        }

        [HttpGet("archived")]
        [ApiVersion("2.0")]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(List<DisciplineResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<DisciplineResponse>>> GetArchivedDisciplinesByGroupIds([FromQuery] GetDisciplineByGroupIdsRequest request, CancellationToken cancellationToken)
        {
            var disciplines = await _disciplineService.GetArchivedDisciplinesByGroupIdsAsync(request.GroupIds.Values, cancellationToken: cancellationToken);
            return Ok(disciplines);
        }

        [HttpPost]
        [ApiVersion("2.0")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(typeof(DisciplineResponse), StatusCodes.Status201Created)]
        public async Task<ActionResult<DisciplineResponse>> CreateDiscipline([FromBody] CreateDisciplineRequest request, CancellationToken cancellationToken)
        {
            var discipline = await _disciplineService.CreateDisciplineAsync(request, cancellationToken: cancellationToken);
            return CreatedAtAction(nameof(GetDiscipline), new { id = discipline.Id }, discipline);
        }

        [HttpPost("bulk")]
        [ApiVersion("2.0")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(typeof(DisciplineResponse), StatusCodes.Status201Created)]
        public async Task<ActionResult<DisciplineResponse>> CreateDisciplineBulk([FromBody] CreateDisciplineBulkRequest request, CancellationToken cancellationToken)
        {
            var disciplines = await _disciplineService.CreateDisciplineAsync(request, cancellationToken: cancellationToken);
            return Created(string.Empty, disciplines);
        }

        [HttpDelete]
        [ApiVersion("2.0")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDiscipline([FromQuery] DeleteDisciplineRequest request, CancellationToken cancellationToken)
        {
            var success = await _disciplineService.DeleteDisciplineAsync(request.Id, cancellationToken: cancellationToken);
            if (!success)
                return NotFound(request.Id);

            return NoContent();
        }
    }
}
