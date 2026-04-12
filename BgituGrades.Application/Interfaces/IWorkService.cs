using BgituGrades.Application.DTOs;

namespace BgituGrades.Application.Interfaces
{
    public interface IWorkService
    {
        Task<List<WorkDTO>> GetAllWorksAsync(CancellationToken cancellationToken);
        Task<WorkDTO> CreateWorkAsync(WorkDTO work, CancellationToken cancellationToken);
        Task<WorkDTO?> GetWorkByIdAsync(int id, CancellationToken cancellationToken);
        Task<WorkDTO> UpdateWorkAsync(WorkDTO work, CancellationToken cancellationToken);
        Task<bool> DeleteWorkAsync(int id, CancellationToken cancellationToken);
    }
}
