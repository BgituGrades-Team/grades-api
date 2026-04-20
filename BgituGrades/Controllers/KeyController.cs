using Asp.Versioning;
using AutoMapper;
using BgituGrades.Application.Interfaces;
using BgituGrades.Application.Models.Key;
using BgituGrades.Application.Models.Student;
using BgituGrades.Domain.Enums;
using BgituGrades.Models.Key;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BgituGrades.API.Controllers
{
    [Route("api/key")]
    [ApiController]
    public class KeyController(IKeyService keyService, IMapper mapper) : ControllerBase
    {
        private readonly IKeyService _keyService = keyService;
        private readonly IMapper _mapper = mapper;

        [HttpGet("all")]
        [ApiVersion("2.0")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(typeof(List<KeyResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<KeyResponse>>> GetAllKeys(CancellationToken cancellationToken)
        {
            var keyDto = await _keyService.GetAllKeysAsync(cancellationToken: cancellationToken);
            var response = _mapper.Map<List<KeyResponse>>(keyDto);
            return Ok(response);
        }

        [HttpPost]
        [ApiVersion("2.0")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(typeof(KeyResponse), StatusCodes.Status201Created)]
        public async Task<ActionResult<KeyResponse>> CreateKey(CreateKeyRequest request, CancellationToken cancellationToken)
        {
            var keyDto = await _keyService.GenerateKeyAsync(request.Role, request.GroupId, cancellationToken: cancellationToken);
            var response = _mapper.Map<KeyResponse>(keyDto);
            return CreatedAtAction(nameof(GetKey), new { key = response.Key }, response);
        }

        [HttpGet]
        [ApiVersion("2.0")]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(KeyResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<KeyResponse>> GetKey([FromHeader(Name = "key")] string key, CancellationToken cancellationToken)
        {
            var keyDto = await _keyService.GetKeyAsync(key, cancellationToken: cancellationToken);
            var response = _mapper.Map<KeyResponse>(keyDto);
            return Ok(response);
        }

        [HttpGet("shared")]
        [ApiVersion("2.0")]
        [Authorize(Policy = "Edit")]
        [ProducesResponseType(typeof(SharedKeyResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<SharedKeyResponse>> CreateSharedKeyV2([FromQuery] CreateSharedKeyRequest request, CancellationToken cancellationToken)
        {
            var key = await _keyService.GenerateKeyAsync(Role.STUDENT, groupId: request.GroupId, cancellationToken: cancellationToken);
            var response = new SharedKeyResponse
            {
                Link = $"{Request.Headers.Origin}/visit?key={key.Key}"
            };
            return Ok(response);
        }

        [HttpDelete]
        [ApiVersion("2.0")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteKey([FromQuery] DeleteKeyRequest request, CancellationToken cancellationToken)
        {
            var success = await _keyService.DeleteKeyAsync(request.DeleteKey, cancellationToken: cancellationToken);
            return success ? NoContent() : NotFound(request.DeleteKey);
        }
    }
}
