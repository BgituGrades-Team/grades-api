using Asp.Versioning;
using AutoMapper;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Interfaces;
using BgituGrades.Application.Models.Class;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BgituGrades.API.Controllers
{
    [Route("api/class")]
    [ApiController]
    public class ClassController(IClassService ClassService, IMapper mapper) : ControllerBase
    {
        private readonly IClassService _classService = ClassService;
        private readonly IMapper _mapper = mapper;

        [HttpPost("bulk")]
        [ApiVersion("2.0")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(typeof(List<ClassResponse>), StatusCodes.Status201Created)]
        public async Task<ActionResult<List<ClassResponse>>> CreateClassBulk([FromBody] CreateClassBulkRequest request, CancellationToken cancellationToken)
        {
            var classDto = _mapper.Map<List<ClassDTO>>(request.Classes);
            classDto = await _classService.CreateClassAsync(classDto, cancellationToken: cancellationToken);
            var response = _mapper.Map<List<ClassResponse>>(classDto);
            return Created(string.Empty, response);
        }
    }
}
