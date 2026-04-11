using BgituGrades.Application.Models.Student;

namespace BgituGrades.Application.Interfaces
{
    public interface IStudentService
    {
        Task<List<StudentResponse>> GetStudentsByGroupAsync(GetStudentsByGroupRequest request, CancellationToken cancellationToken);
        Task<List<StudentResponse>> GetArchivedStudentsByGroupAsync(GetStudentsByGroupRequest request, CancellationToken cancellationToken);
        Task<StudentResponse> CreateStudentAsync(CreateStudentRequest request, CancellationToken cancellationToken);
        Task<StudentResponse?> GetStudentByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> UpdateStudentAsync(UpdateStudentRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteStudentAsync(int id, CancellationToken cancellationToken);
        Task<ImportResult> ImportStudentsFromXlsxAsync(Stream streamFile, CancellationToken cancellationToken);
        Task DeleteAllAsync(CancellationToken cancellationToken);
    }
}
