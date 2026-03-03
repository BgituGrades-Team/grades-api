using BgituGrades.Data;
using BgituGrades.Entities;
using BgituGrades.Models.Class;
using BgituGrades.Models.Mark;
using BgituGrades.Models.Presence;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BgituGrades.Repositories
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<IEnumerable<Student>> GetStudentsByGroupAsync(int groupId);
        Task<IEnumerable<Student>> GetStudentsByGroupIdsAsync(int[] groupIds);
        Task<IEnumerable<FullGradeMarkResponse>> GetMarksGrade(IEnumerable<Work> works, int groupId, int disciplineId);
        Task<IEnumerable<FullGradePresenceResponse>> GetPresenseGrade(IEnumerable<ClassDateResponse> scheduleDates, int groupId, int disciplineId);
        Task<Student> CreateStudentAsync(Student entity);
        Task<Student?> GetByIdAsync(int id);
        Task<bool> UpdateStudentAsync(Student entity);
        Task<bool> DeleteStudentAsync(int id);
        Task<IEnumerable<Student>> GetStudentsByIdsAsync(int[] studentIds);
    }

    public class StudentRepository(IDbContextFactory<AppDbContext> contextFactory) : IStudentRepository
    {

        public async Task<Student> CreateStudentAsync(Student entity)
        {
            using var context = await contextFactory.CreateDbContextAsync();
            await context.Students.AddAsync(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            using var context = await contextFactory.CreateDbContextAsync();
            var result = await context.Students.Where(s => s.Id == id).ExecuteDeleteAsync();
            return result > 0;
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            using var context = await contextFactory.CreateDbContextAsync();
            var entity = await context.Students.FindAsync(id);
            return entity;
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            using var context = await contextFactory.CreateDbContextAsync();
            var entities = await context.Students
                .AsNoTracking()
                .ToListAsync();
            return entities;
        }

        public async Task<IEnumerable<Student>> GetStudentsByGroupAsync(int groupId)
        {
            using var context = await contextFactory.CreateDbContextAsync();
            var entities = await context.Students
                .Where(s => s.GroupId == groupId)
                .AsNoTracking()
                .ToListAsync();
            return entities;
        }

        public async Task<IEnumerable<FullGradePresenceResponse>> GetPresenseGrade(IEnumerable<ClassDateResponse> scheduleDates, int groupId, int disciplineId)
        {
            using var context = await contextFactory.CreateDbContextAsync();
            var studentsWithPresence = await context.Students
                .Where(s => s.GroupId == groupId)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    PresencesByDate = s.Presences
                        .Where(p => p.DisciplineId == disciplineId)
                        .ToDictionary(p => p.Date, p => p.IsPresent)
                })
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .ToListAsync();

            var scheduleDatesList = scheduleDates.ToList();
            var result = studentsWithPresence.Select(s => new FullGradePresenceResponse
            {
                StudentId = s.Id,
                Name = s.Name,
                Presences = scheduleDatesList.Select(date => new GradePresenceResponse
                {
                    ClassId = date.Id,
                    ClassType = date.ClassType,
                    Date = date.Date,
                    IsPresent = s.PresencesByDate.TryGetValue(date.Date, out var presence) 
                        ? presence 
                        : PresenceType.PRESENT
                }).ToList()
            });

            return result;
        }

        public async Task<IEnumerable<FullGradeMarkResponse>> GetMarksGrade(IEnumerable<Work> works, int groupId, int disciplineId)
        {
            using var context = await contextFactory.CreateDbContextAsync();
            var studentsWithMarks = await context.Students
                .Where(s => s.GroupId == groupId)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    MarksByWorkId = s.Marks
                        .Where(m => m.Work.DisciplineId == disciplineId)
                        .ToDictionary(m => m.WorkId, m => m.Value)
                })
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .ToListAsync();

            var worksList = works.ToList();
            var result = studentsWithMarks.Select(s => new FullGradeMarkResponse
            {
                StudentId = s.Id,
                Name = s.Name,
                Marks = worksList.Select(work => new GradeMarkResponse
                {
                    WorkId = work.Id,
                    Name = work.Name,
                    Value = s.MarksByWorkId.TryGetValue(work.Id, out var mark) ? mark : ""
                }).ToList()
            });

            return result;
        }

        public async Task<bool> UpdateStudentAsync(Student entity)
        {
            using var context = await contextFactory.CreateDbContextAsync();
            context.Update(entity);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Student>> GetStudentsByIdsAsync(int[] studentIds)
        {
            using var context = await contextFactory.CreateDbContextAsync();
            return await context.Students
                .AsNoTracking()
                .Where(s => studentIds.Contains(s.Id))
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetStudentsByGroupIdsAsync(int[] groupIds)
        {
            using var context = await contextFactory.CreateDbContextAsync();
            var entities = await context.Students
                .AsNoTracking()
                .Where(s =>  groupIds.Contains(s.GroupId))
                .ToListAsync();
            return entities;
        }
    }
}
