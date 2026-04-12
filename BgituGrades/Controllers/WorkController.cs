using Asp.Versioning;
using AutoMapper;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Interfaces;
using BgituGrades.Application.Models.Student;
using BgituGrades.Application.Models.Work;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BgituGrades.Controllers
{
    [Route("api/work")]
    [ApiVersion("2.0")]
    [ApiController]
    public class WorkController(IWorkService WorkService, IMapper mapper) : ControllerBase
    {
        private readonly IWorkService _workService = WorkService;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(List<WorkResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<WorkResponse>>> GetAllWorks(CancellationToken cancellationToken)
        {
            var workDto = await _workService.GetAllWorksAsync(cancellationToken: cancellationToken);
            var response = _mapper.Map<List<WorkResponse>>(workDto);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Policy = "Edit")]
        [ProducesResponseType(typeof(WorkResponse), StatusCodes.Status201Created)]
        public async Task<ActionResult<WorkResponse>> CreateWork([FromBody] CreateWorkRequest request, CancellationToken cancellationToken)
        {
            var workDto = _mapper.Map<WorkDTO>(request);
            workDto = await _workService.CreateWorkAsync(workDto, cancellationToken: cancellationToken);
            var response = _mapper.Map<WorkResponse>(workDto);
            return CreatedAtAction(nameof(GetWork), new { id = response.Id }, response);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(WorkResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkResponse>> GetWork([FromRoute] int id, CancellationToken cancellationToken)
        {
            var workDto = await _workService.GetWorkByIdAsync(id, cancellationToken: cancellationToken);
            return workDto == null ? NotFound(id) : Ok(_mapper.Map<WorkResponse>(workDto));
        }

        [HttpPut]
        [Authorize(Policy = "Edit")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateWork([FromBody] UpdateWorkRequest request, CancellationToken cancellationToken)
        {
            var workDto = _mapper.Map<WorkDTO>(request);
            workDto = await _workService.UpdateWorkAsync(workDto, cancellationToken: cancellationToken);
            return workDto == null ? NotFound(request.Id) : NoContent();
        }

        [HttpDelete]
        [Authorize(Policy = "Edit")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteWork([FromQuery] DeleteWorkRequest request, CancellationToken cancellationToken)
        {
            var success = await _workService.DeleteWorkAsync(request.Id, cancellationToken: cancellationToken);
            return success ? NotFound(request.Id) : NoContent();
        }
    }
}
