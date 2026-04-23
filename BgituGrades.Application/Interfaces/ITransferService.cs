using BgituGrades.Application.DTOs;
using BgituGrades.Application.Models.Transfer;

namespace BgituGrades.Application.Interfaces
{
    public interface ITransferService
    {
        Task<TransferResponse> CreateTransferAsync(CreateTransferRequest request, CancellationToken cancellationToken);
        Task<TransferResponse?> GetTransferByClassIdAndDateAsync(int classId, DateOnly originalDate, CancellationToken cancellationToken);
        Task<TransferResponse?> GetTransferByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<TransferResponse>> GetTransfersByGroupAndDisciplineAsync(int groupId, int disciplineId, CancellationToken cancellationToken);
        Task<bool> UpdateTransferAsync(UpdateTransferRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteTransferAsync(int id, CancellationToken cancellationToken);
    }
}
