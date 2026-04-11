using BgituGrades.Application.DTOs;
using BgituGrades.Application.Models.Transfer;

namespace BgituGrades.Application.Interfaces
{
    public interface ITransferService
    {
        Task<List<TransferResponse>> GetAllTransfersAsync(CancellationToken cancellationToken);
        Task<TransferResponse> CreateTransferAsync(CreateTransferRequest request, CancellationToken cancellationToken);
        Task<TransferResponse?> GetTransferByClassIdAsync(int classId, CancellationToken cancellationToken);
        Task<TransferResponse?> GetTransferByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<TransferResponse>> GetTransfersByGroupAndDisciplineAsync(int groupId, int disciplineId, CancellationToken cancellationToken);
        Task<bool> UpdateTransferAsync(UpdateTransferRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteTransferAsync(int id, CancellationToken cancellationToken);
        Task<List<TransferDTO>> GetAllTransfersDtoAsync(CancellationToken cancellationToken);
        Task<TransferDTO?> GetTransferDtoByIdAsync(int id, CancellationToken cancellationToken);
    }
}
