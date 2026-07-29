using Asp.Versioning;
using AutoMapper;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Interfaces;
using BgituGrades.Application.Models.Student;
using BgituGrades.Application.Models.Work;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BgituGrades.API.Controllers
{
    [Route("api/works")]
    [ApiVersion("2.0")]
    [ApiController]
    public class WorkController(IWorkService workService, IMapper mapper) : ControllerBase
    {
        private readonly IWorkService _workService = workService;
        private readonly IMapper _mapper = mapper;

        [HttpPost]
        [Authorize(Policy = "Edit")]
        [EndpointSummary("Создать работу")]
        [ProducesResponseType(typeof(WorkResponse), StatusCodes.Status201Created)]
        public async Task<ActionResult<WorkResponse>> CreateWork([FromBody] CreateWorkRequest request, CancellationToken cancellationToken)
        {
            var workDto = _mapper.Map<WorkDTO>(request);
            workDto = await _workService.CreateWorkAsync(workDto, cancellationToken: cancellationToken);
            var response = _mapper.Map<WorkResponse>(workDto);
            return Created(string.Empty, response);
        }

        [HttpPut]
        [Authorize(Policy = "Edit")]
        [EndpointSummary("Обновить работу")]
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
        [EndpointSummary("Удалить работу")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteWork([FromQuery] DeleteWorkRequest request, CancellationToken cancellationToken)
        {
            var success = await _workService.DeleteWorkAsync(request.Id, cancellationToken: cancellationToken);
            return success ? NoContent() : NotFound(request.Id);
        }
    }
}
