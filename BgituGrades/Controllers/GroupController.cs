using Asp.Versioning;
using AutoMapper;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Interfaces;
using BgituGrades.Application.Models.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BgituGrades.API.Controllers
{
    [Route("api/group")]
    [ApiController]
    public class GroupController(IGroupService groupService, IMapper mapper) : ControllerBase
    {
        private readonly IGroupService _groupService = groupService;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [ApiVersion("2.0")]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(List<GroupResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GroupResponse>>> GetGroupsByDiscipline([FromQuery] GetGroupsByDisciplineRequest request, CancellationToken cancellationToken)
        {
            var groupDto = await _groupService.GetGroupsByDisciplineAsync(request.DisciplineId, cancellationToken: cancellationToken);
            var response = _mapper.Map<List<GroupResponse>>(groupDto);
            return Ok(response);
        }

        [HttpGet("courses")]
        [ApiVersion("2.0")]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(List<int>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<int>>> GetCourses(CancellationToken cancellationToken)
        {
            var courses = await _groupService.GetCoursesAsync(cancellationToken: cancellationToken);
            return Ok(courses);
        }

        [HttpGet("archived/courses")]
        [ApiVersion("2.0")]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(List<int>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<int>>> GetArchivedCoursesByPeriod(
            [FromQuery] GetByPeriodRequest request, CancellationToken cancellationToken)
        {
            var groups = await _groupService.GetArchivedCoursesByPeriodAsync(request.Year, request.Semester, cancellationToken: cancellationToken);
            return Ok(groups);
        }

        [HttpGet("all")]
        [ApiVersion("2.0")]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(List<GroupResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GroupResponse>>> GetAllGroups(CancellationToken cancellationToken)
        {
            var isStudent = User.IsInRole("STUDENT");

            var groupIdClaim = User.FindFirst("group_id")?.Value;

            if (isStudent && groupIdClaim != null)
            {
                var groupId = int.Parse(groupIdClaim);
                var groupDto = await _groupService.GetGroupByIdAsync(groupId, cancellationToken);
                var singleResponse = _mapper.Map<GroupResponse>(groupDto);
                return Ok(new List<GroupResponse> { singleResponse });
            }

            var groupsDto = await _groupService.GetAllAsync(cancellationToken: cancellationToken);
            var response = _mapper.Map<List<GroupResponse>>(groupsDto);
            return Ok(response);
        }

        [HttpGet("archived")]
        [ApiVersion("2.0")]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(List<ArchivedGroupResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ArchivedGroupResponse>>> GetArchivedGroups([FromQuery] GetByPeriodRequest request, CancellationToken cancellationToken)
        {
            var groupDto = await _groupService.GetArchivedGroupsByPeriodAsync(semester: request.Semester, year: request.Year, cancellationToken: cancellationToken);
            var response = _mapper.Map<List<ArchivedGroupResponse>>(groupDto);
            return Ok(response);
        }

        [HttpGet("by_courses")]
        [ApiVersion("2.0")]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(List<GroupResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GroupResponse>>> GetGroupsByCourses([FromQuery] GetByCoursesRequest request, CancellationToken cancellationToken)
        {
            var groupDto = await _groupService.GetGroupsByCoursesAsync(request.Courses!.Values, cancellationToken: cancellationToken);
            var response = _mapper.Map<List<GroupResponse>>(groupDto);
            return Ok(response);
        }

        [HttpPost("bulk")]
        [ApiVersion("2.0")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(typeof(List<GroupResponse>), StatusCodes.Status201Created)]
        public async Task<ActionResult<List<GroupResponse>>> CreateGroupBulk([FromBody] CreateGroupBulkRequest request, CancellationToken cancellationToken)
        {
            var groupDto = _mapper.Map<List<GroupDTO>>(request.Groups);
            groupDto = await _groupService.CreateGroupAsync(groupDto, cancellationToken: cancellationToken);
            var response = _mapper.Map<List<GroupResponse>>(groupDto);
            return Created(string.Empty, response);
        }
    }
}
