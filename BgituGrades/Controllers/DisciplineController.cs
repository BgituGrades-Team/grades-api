using Asp.Versioning;
using AutoMapper;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Interfaces;
using BgituGrades.Application.Models.Discipline;
using BgituGrades.Application.Models.Student;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BgituGrades.API.Controllers
{
    [Route("api/discipline")]
    [ApiController]
    public class DisciplineController(IDisciplineService DisciplineService, IMapper mapper) : ControllerBase
    {
        private readonly IDisciplineService _disciplineService = DisciplineService;
        private readonly IMapper _mapper = mapper;

        [HttpGet("all")]
        [ApiVersion("2.0")]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(List<DisciplineResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<DisciplineResponse>>> GetDisciplines(CancellationToken cancellationToken)
        {
            var disciplinesDto = await _disciplineService.GetAllDisciplinesAsync(cancellationToken: cancellationToken);
            var response = _mapper.Map<List<DisciplineResponse>>(disciplinesDto);
            return Ok(response);
        }

        [HttpGet]
        [ApiVersion("2.0")]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(List<DisciplineResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<DisciplineResponse>>> GetDisciplinesByGroupIds([FromQuery] GetDisciplineByGroupIdsRequest request, CancellationToken cancellationToken)
        {
            var disciplinesDto = await _disciplineService.GetDisciplineByGroupIdAsync(request.GroupIds.Values, cancellationToken: cancellationToken);
            var response = _mapper.Map<List<DisciplineResponse>>(disciplinesDto);
            return Ok(response);
        }

        [HttpGet("archived")]
        [ApiVersion("2.0")]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(List<DisciplineResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<DisciplineResponse>>> GetArchivedDisciplinesByGroupIds([FromQuery] GetDisciplineByGroupIdsRequest request, CancellationToken cancellationToken)
        {
            var disciplinesDto = await _disciplineService.GetArchivedDisciplinesByGroupIdsAsync(request.GroupIds.Values, cancellationToken: cancellationToken);
            var response = _mapper.Map<List<DisciplineResponse>>(disciplinesDto);
            return Ok(response);
        }

        [HttpPost]
        [ApiVersion("2.0")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(typeof(DisciplineResponse), StatusCodes.Status201Created)]
        public async Task<ActionResult<DisciplineResponse>> CreateDiscipline([FromBody] CreateDisciplineRequest request, CancellationToken cancellationToken)
        {
            var disciplineDto = _mapper.Map<DisciplineDTO>(request);
            disciplineDto = await _disciplineService.CreateDisciplineAsync(disciplineDto, cancellationToken: cancellationToken);
            var response = _mapper.Map<DisciplineResponse>(disciplineDto);
            return Created(string.Empty, response);
        }

        [HttpPost("bulk")]
        [ApiVersion("2.0")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(typeof(List<DisciplineResponse>), StatusCodes.Status201Created)]
        public async Task<ActionResult<List<DisciplineResponse>>> CreateDisciplineBulk([FromBody] CreateDisciplineBulkRequest request, CancellationToken cancellationToken)
        {
            var disciplineDto = _mapper.Map<List<DisciplineDTO>>(request.Disciplines);
            disciplineDto = await _disciplineService.CreateDisciplineAsync(disciplineDto, cancellationToken: cancellationToken);
            var response = _mapper.Map<List<DisciplineResponse>>(disciplineDto);
            return Created(string.Empty, response);
        }

        [HttpDelete]
        [ApiVersion("2.0")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDiscipline([FromQuery] DeleteDisciplineRequest request, CancellationToken cancellationToken)
        {
            var success = await _disciplineService.DeleteDisciplineAsync(request.Id, cancellationToken: cancellationToken);
            return success ? NoContent() : NotFound(request.Id);
        }
    }
}
