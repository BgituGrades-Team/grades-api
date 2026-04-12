using Asp.Versioning;
using BgituGrades.Application.Interfaces;
using BgituGrades.Application.Models.Class;
using BgituGrades.Application.Models.Student;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BgituGrades.API.Controllers
{
    [Route("api/class")]
    [ApiController]
    public class ClassController(IClassService ClassService) : ControllerBase
    {
        private readonly IClassService _classService = ClassService;

        [HttpPost]
        [ApiVersion("2.0")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(typeof(ClassResponse), StatusCodes.Status201Created)]
        public async Task<ActionResult<ClassResponse>> CreateClass([FromBody] CreateClassRequest request, CancellationToken cancellationToken)
        {
            var _class = await _classService.CreateClassAsync(request, cancellationToken: cancellationToken);
            return Created(string.Empty, _class);
        }

        [HttpPost("bulk")]
        [ApiVersion("2.0")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(typeof(List<ClassResponse>), StatusCodes.Status201Created)]
        public async Task<ActionResult<List<ClassResponse>>> CreateClassBulk([FromBody] CreateClassBulkRequest request, CancellationToken cancellationToken)
        {
            var _classes = await _classService.CreateClassAsync(request, cancellationToken: cancellationToken);
            return Created(string.Empty, _classes);
        }

        [HttpDelete]
        [ApiVersion("2.0")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteClass([FromQuery] int id, CancellationToken cancellationToken)
        {
            var success = await _classService.DeleteClassAsync(id, cancellationToken: cancellationToken);
            if (!success)
                return NotFound(id);

            return NoContent();
        }
    }
}
