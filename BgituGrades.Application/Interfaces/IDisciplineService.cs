using BgituGrades.Application.DTOs;

namespace BgituGrades.Application.Interfaces
{
    public interface IDisciplineService
    {
        Task<List<DisciplineDTO>> GetAllDisciplinesAsync(CancellationToken cancellationToken);
        Task<DisciplineDTO> CreateDisciplineAsync(DisciplineDTO request, CancellationToken cancellationToken);
        Task<List<DisciplineDTO>> CreateDisciplineAsync(IEnumerable<DisciplineDTO> request, CancellationToken cancellationToken);
        Task<List<DisciplineDTO>> GetDisciplineByGroupIdAsync(IEnumerable<int> groupIds, CancellationToken cancellationToken);
        Task<bool> DeleteDisciplineAsync(int id, CancellationToken cancellationToken);
        Task<List<DisciplineDTO>> GetArchivedDisciplinesByGroupIdsAsync(IEnumerable<int> groupIds, CancellationToken cancellationToken);
    }
}
