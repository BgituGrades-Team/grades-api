using Asp.Versioning;
using BgituGrades.Models.Student;
using BgituGrades.Models.Transfer;
using BgituGrades.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BgituGrades.Controllers
{
    [Route("api/transfer")]
    [ApiController]
    public class TransferController(ITransferService TransferService) : ControllerBase
    {
        private readonly ITransferService _transferService = TransferService;

        [HttpGet("all")]
        [ApiVersion("2.0")]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(IEnumerable<TransferResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TransferResponse>>> GetAllTransfers(CancellationToken cancellationToken)
        {
            var transfers = await _transferService.GetAllTransfersAsync(cancellationToken: cancellationToken);
            return Ok(transfers);
        }

        [HttpGet]
        [ApiVersion("2.0")]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(TransferResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TransferResponse>> GetTransfers([FromQuery] GetTransferRequest request, CancellationToken cancellationToken)
        {
            var transfer = await _transferService.GetTransferByClassIdAsync(request.ClassId, cancellationToken: cancellationToken);

            return transfer == null ? NotFound() : Ok(transfer);
        }

        [HttpPost]
        [ApiVersion("2.0")]
        [Authorize(Policy = "Edit")]
        [ProducesResponseType(typeof(TransferResponse), StatusCodes.Status201Created)]
        public async Task<ActionResult<TransferResponse>> CreateTransfer([FromBody] CreateTransferRequest request, CancellationToken cancellationToken)
        {
            var transfer = await _transferService.CreateTransferAsync(request, cancellationToken: cancellationToken);
            return CreatedAtAction(nameof(GetTransfer), new { id = transfer.Id }, transfer);
        }

        [HttpGet("{id}")]
        [ApiVersion("2.0")]
        [Authorize(Policy = "ViewOnly")]
        [ProducesResponseType(typeof(TransferResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TransferResponse>> GetTransfer([FromRoute] int id, CancellationToken cancellationToken)
        {
            var transfer = await _transferService.GetTransferByIdAsync(id, cancellationToken: cancellationToken);
            if (transfer == null)
                return NotFound(id);

            return Ok(transfer);
        }

        [HttpPut]
        [ApiVersion("2.0")]
        [Authorize(Policy = "Edit")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTransfer([FromBody] UpdateTransferRequest request, CancellationToken cancellationToken)
        {
            var success = await _transferService.UpdateTransferAsync(request, cancellationToken: cancellationToken);
            if (!success)
                return NotFound(request.Id);

            return NoContent();
        }

        [HttpDelete]
        [ApiVersion("2.0")]
        [Authorize(Policy = "Edit")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTransfer([FromQuery] int id, CancellationToken cancellationToken)
        {
            var success = await _transferService.DeleteTransferAsync(id, cancellationToken: cancellationToken);
            if (!success)
                return NotFound(id);

            return NoContent();
        }
    }
}
