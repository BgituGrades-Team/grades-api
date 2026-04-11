using BgituGrades.Application.DTOs;
using BgituGrades.Application.Models.Discipline;

namespace BgituGrades.Application.Interfaces
{
    public interface IDisciplineService
    {
        Task<List<DisciplineResponse>> GetAllDisciplinesAsync(CancellationToken cancellationToken);
        Task<DisciplineResponse> CreateDisciplineAsync(CreateDisciplineRequest request, CancellationToken cancellationToken);
        Task<List<DisciplineResponse>> CreateDisciplineAsync(CreateDisciplineBulkRequest request, CancellationToken cancellationToken);
        Task<DisciplineResponse?> GetDisciplineByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<DisciplineResponse>?> GetDisciplineByGroupIdAsync(IEnumerable<int> groupId, CancellationToken cancellationToken);
        Task<bool> UpdateDisciplineAsync(UpdateDisciplineRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteDisciplineAsync(int id, CancellationToken cancellationToken);
        Task<List<DisciplineDTO>> GetAllDisciplinesDtoAsync(CancellationToken cancellationToken);
        Task<List<DisciplineDTO>?> GetDisciplinesDtoByGroupIdAsync(int groupId, CancellationToken cancellationToken);
        Task<List<DisciplineResponse>?> GetArchivedDisciplinesByGroupIdsAsync(IEnumerable<int> groupIds, CancellationToken cancellationToken);
        Task<DisciplineDTO?> GetDisciplineDtoByIdAsync(int id, CancellationToken cancellationToken);
    }
}
