using BgituGrades.Application.DTOs;
using BgituGrades.Application.Models.Mark;

namespace BgituGrades.Application.Interfaces
{
    public interface IMarkService
    {
        Task<List<MarkDTO>> GetAllMarksAsync(CancellationToken cancellationToken);
        Task<MarkDTO> CreateMarkAsync(CreateMarkRequest request, CancellationToken cancellationToken);
        Task<List<MarkDTO>> GetMarksByDisciplineAndGroupAsync(GetMarksByDisciplineAndGroupRequest request, CancellationToken cancellationToken);
        Task<bool> UpdateMarkAsync(UpdateMarkRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteMarkByStudentAndWorkAsync(DeleteMarkByStudentAndWorkRequest request, CancellationToken cancellationToken);
        Task<MarkDTO> UpdateOrCreateMarkAsync(UpdateMarkGradeRequest request, CancellationToken cancellationToken);
        Task<List<MarkDTO>> GetAllMarksDtoAsync(CancellationToken cancellationToken);
        Task<MarkDTO?> GetMarkDtoByIdAsync(int id, CancellationToken cancellationToken);
    }
}
