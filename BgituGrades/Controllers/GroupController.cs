using Asp.Versioning;
using AutoMapper;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Interfaces;
using BgituGrades.Application.Models.Group;
using BgituGrades.Application.Models.Student;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BgituGrades.API.Controllers
{
    [Route("api/group")]
    [ApiController]
    [ApiVersion("2.0")]
    public class GroupController(IGroupService groupService, IMapper mapper, ILogger<GroupController> logger) : ControllerBase
    {
        private readonly IGroupService _groupService = groupService;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GroupController> _logger = logger;

        [HttpGet]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(List<GroupResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GroupResponse>>> GetGroupsByDiscipline([FromQuery] GetGroupsByDisciplineRequest request, CancellationToken cancellationToken)
        {
            var groupDto = await _groupService.GetGroupsByDisciplineAsync(request.DisciplineId, cancellationToken: cancellationToken);
            var response = _mapper.Map<List<GroupResponse>>(groupDto);
            return Ok(response);
        }

        [HttpGet("courses")]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(List<int>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<int>>> GetCoursesByPeriod(CancellationToken cancellationToken)
        {
            var courses = await _groupService.GetCoursesAsync(cancellationToken: cancellationToken);
            return Ok(courses);
        }

        [HttpGet("archived/courses")]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(List<int>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<int>>> GetArchivedCoursesByPeriod(
            [FromQuery] GetByPeriodRequest request, CancellationToken cancellationToken)
        {
            var groups = await _groupService.GetArchivedCoursesByPeriodAsync(request.Year, request.Semester, cancellationToken: cancellationToken);
            return Ok(groups);
        }

        [HttpGet("all")]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(List<GroupResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GroupResponse>>> GetAllGroups(CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            var isStudent = User.IsInRole("STUDENT");
            _logger.LogInformation("IsInRole: {ms}ms", sw.ElapsedMilliseconds);
            sw.Restart();

            var groupIdClaim = User.FindFirst("group_id")?.Value;

            if (isStudent && groupIdClaim != null)
            {
                var groupId = int.Parse(groupIdClaim);
                var groupDto = await _groupService.GetGroupByIdAsync(groupId, cancellationToken);
                var singleResponse = _mapper.Map<GroupResponse>(groupDto);
                return Ok(new List<GroupResponse> { singleResponse });
            }

            var groupsDto = await _groupService.GetAllAsync(cancellationToken: cancellationToken);
            _logger.LogInformation("GetAllAsync: {ms}ms", sw.ElapsedMilliseconds);
            sw.Restart();
            var response = _mapper.Map<List<GroupResponse>>(groupsDto);
            _logger.LogInformation("Map: {ms}ms", sw.ElapsedMilliseconds);
            return Ok(response);
        }

        [HttpGet("archived")]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(List<ArchivedGroupResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ArchivedGroupResponse>>> GetArchivedGroups([FromQuery] GetByPeriodRequest request, CancellationToken cancellationToken)
        {
            var groupDto = await _groupService.GetArchivedGroupsByPeriodAsync(semester: request.Semester, year: request.Year, cancellationToken: cancellationToken);
            var response = _mapper.Map<List<ArchivedGroupResponse>>(groupDto);
            return Ok(response);
        }

        [HttpGet("by_courses")]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(List<GroupResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GroupResponse>>> GetGroupsByCourses([FromQuery] GetByCoursesRequest request, CancellationToken cancellationToken)
        {
            var groupDto = await _groupService.GetGroupsByCoursesAsync(request.Courses!.Values, cancellationToken: cancellationToken);
            var response = _mapper.Map<List<GroupResponse>>(groupDto);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status201Created)]
        public async Task<ActionResult<GroupResponse>> CreateGroup([FromBody] CreateGroupRequest request, CancellationToken cancellationToken)
        {
            var groupDto = _mapper.Map<GroupDTO>(request);
            groupDto = await _groupService.CreateGroupAsync(groupDto, cancellationToken: cancellationToken);
            var response = _mapper.Map<GroupResponse>(groupDto);
            return CreatedAtAction(nameof(GetGroupsByDiscipline), new { id = response.Id }, response);
        }

        [HttpPost("bulk")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(typeof(List<GroupResponse>), StatusCodes.Status201Created)]
        public async Task<ActionResult<List<GroupResponse>>> CreateGroupBulk([FromBody] CreateGroupBulkRequest request, CancellationToken cancellationToken)
        {
            var groupDto = _mapper.Map<List<GroupDTO>>(request.Groups);
            groupDto = await _groupService.CreateGroupAsync(groupDto, cancellationToken: cancellationToken);
            var response = _mapper.Map<List<GroupResponse>>(groupDto);
            return Created(string.Empty, response);
        }

        [HttpDelete]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteGroup([FromQuery] DeleteGroupRequest request, CancellationToken cancellationToken)
        {
            var success = await _groupService.DeleteGroupAsync(request.Id, cancellationToken: cancellationToken);
            return success ? NoContent() : NotFound(request.Id);
        }
    }
}
