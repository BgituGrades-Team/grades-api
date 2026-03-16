using Asp.Versioning;
using BgituGrades.Models.Student;
using BgituGrades.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BgituGrades.Controllers
{
    [Route("api/student")]
    [ApiController]
    public class StudentController(IStudentService studentService) : ControllerBase
    {
        private readonly IStudentService _studentService = studentService;

        [HttpGet()]
        [ApiVersion("2.0")]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(IEnumerable<StudentResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<StudentResponse>>> GetStudents([FromQuery] GetStudentsByGroupRequest request, CancellationToken cancellationToken)
        {
            var students = await _studentService.GetStudentsByGroupAsync(request, cancellationToken: cancellationToken);
            return Ok(students);
        }

        [HttpPost]
        [ApiVersion("2.0")]
        [Authorize(Policy = "Edit")]
        [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status201Created)]
        public async Task<ActionResult<StudentResponse>> CreateStudent([FromBody] CreateStudentRequest request, CancellationToken cancellationToken)
        {
            var student = await _studentService.CreateStudentAsync(request, cancellationToken: cancellationToken);
            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
        }

        [HttpGet("{id}")]
        [ApiVersion("1.0")]
        [Obsolete("deprecated")]
        [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentResponse>> GetStudent([FromRoute] int id, CancellationToken cancellationToken)
        {
            var student = await _studentService.GetStudentByIdAsync(id, cancellationToken: cancellationToken);
            if (student == null)
                return NotFound(id);
            return Ok(student);
        }

        [HttpPut]
        [ApiVersion("2.0")]
        [Authorize(Policy = "Edit")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStudent([FromBody] UpdateStudentRequest request, CancellationToken cancellationToken)
        {
            var success = await _studentService.UpdateStudentAsync(request, cancellationToken: cancellationToken);
            if (!success)
                return NotFound(request.Id);

            return NoContent();
        }

        [HttpDelete]
        [ApiVersion("2.0")]
        [Authorize(Policy = "Edit")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStudent([FromQuery] DeleteStudentRequest request, CancellationToken cancellationToken)
        {
            var success = await _studentService.DeleteStudentAsync(request.Id, cancellationToken: cancellationToken);
            if (!success)
                return NotFound(request.Id);

            return NoContent();
        }
    }
}
