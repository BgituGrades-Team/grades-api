using BgituGrades.Application.Models.Work;

namespace BgituGrades.Application.Interfaces
{
    public interface IWorkService
    {
        Task<List<WorkResponse>> GetAllWorksAsync(CancellationToken cancellationToken);
        Task<WorkResponse> CreateWorkAsync(CreateWorkRequest request, CancellationToken cancellationToken);
        Task<WorkResponse?> GetWorkByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> UpdateWorkAsync(UpdateWorkRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteWorkAsync(int id, CancellationToken cancellationToken);
    }
}
