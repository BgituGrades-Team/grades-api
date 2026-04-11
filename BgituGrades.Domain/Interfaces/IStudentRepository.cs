using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Models;

namespace BgituGrades.Domain.Interfaces
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAllStudentsAsync(CancellationToken cancellationToken);
        Task<List<Student>> GetStudentsByGroupAsync(int groupId, CancellationToken cancellationToken);
        Task<List<Student>> GetStudentsByGroupIdsAsync(IEnumerable<int> groupIds, CancellationToken cancellationToken);
        Task<List<StudentMarkResult>> GetMarksGrade(IEnumerable<Work> works, int groupId, int disciplineId, CancellationToken cancellationToken);
        Task<List<StudentPresenceResult>> GetPresenseGrade(IEnumerable<ScheduleDate> scheduleDates, int groupId, int disciplineId, CancellationToken cancellationToken);
        Task<Student> CreateStudentAsync(Student entity, CancellationToken cancellationToken);
        Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> UpdateStudentAsync(Student entity, CancellationToken cancellationToken);
        Task<bool> DeleteStudentAsync(int id, CancellationToken cancellationToken);
        Task<List<Student>> GetArchivedByGroupIdsAsync(IEnumerable<int> groupIds, CancellationToken cancellationToken);
        Task<List<Student>> GetStudentsByIdsAsync(IEnumerable<int> studentIds, CancellationToken cancellationToken);
        Task BulkInsertAsync(IEnumerable<Student> students, CancellationToken cancellationToken);
        Task DeleteByIdsAsync(IEnumerable<int> studentsIds, CancellationToken cancellationToken);
        Task DeleteAllAsync(CancellationToken cancellationToken);
    }
}
