using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Enums;
using BgituGrades.Domain.Interfaces;
using BgituGrades.Domain.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace BgituGrades.Infrastructure.Persistence.Repositories
{
    

    public class StudentRepository(IDbContextFactory<AppDbContext> contextFactory) : IStudentRepository
    {

        public async Task<Student> CreateStudentAsync(Student entity, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            await context.Students.AddAsync(entity, cancellationToken: cancellationToken);
            await context.SaveChangesAsync(cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<bool> DeleteStudentAsync(int id, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var result = await context.Students.Where(s => s.Id == id).ExecuteDeleteAsync(cancellationToken: cancellationToken);
            return result > 0;
        }

        public async Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var entity = await context.Students.FindAsync([id], cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<List<Student>> GetAllStudentsAsync(CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var entities = await context.Students
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
            return entities;
        }

        public async Task<List<Student>> GetStudentsByGroupAsync(int groupId, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var entities = await context.Students
                .Where(s => s.GroupId == groupId)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
            return entities;
        }

        public async Task<List<StudentPresenceResult>> GetPresenseGrade(IEnumerable<ScheduleDate> scheduleDates,
            int groupId, int disciplineId, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var students = await context.Students
                .Where(s => s.GroupId == groupId)
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var studentIds = students.Select(s => s.Id).ToList();

            var allPresences = await context.Presences
                .Where(p => p.DisciplineId == disciplineId && studentIds.Contains(p.StudentId))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var presenceLookup = allPresences.ToLookup(p => (p.StudentId, p.ClassId, p.Date));

            var result = students.Select(s => new StudentPresenceResult
            {
                StudentId = s.Id,
                Name = s.Name,
                Presences = scheduleDates.Select(date =>
                {
                    var isPresent = presenceLookup[(s.Id, date.Id, date.OriginalDate)].Select(p => p.IsPresent).FirstOrDefault(PresenceType.PRESENT);

                    return new PresenceEntry
                    {
                        ClassId = date.Id,
                        ClassType = date.ClassType,
                        Date = date.Date,
                        OriginalDate = date.OriginalDate,
                        IsPresent = isPresent
                    };
                }).ToList()
            }).ToList();

            return result;
        }

        public async Task<List<StudentMarkResult>> GetMarksGrade(IEnumerable<Work> works,
            int groupId, int disciplineId, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var studentsWithMarks = await context.Students
                .Where(s => s.GroupId == groupId)
                    .Include(s => s.Marks!)
                        .ThenInclude(m => m.Work)
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync(cancellationToken: cancellationToken);

            var marksByStudent = studentsWithMarks.Select(s => new
            {
                s.Id,
                s.Name,
                MarksByWorkId = s.Marks!
                    .Where(m => m.Work!.DisciplineId == disciplineId)
                    .ToLookup(m => m.WorkId, m => new { m.Value, m.IsOverdue })
            }).ToList();

            var worksList = works.ToList();
            var result = marksByStudent.Select(s => new StudentMarkResult
            {
                StudentId = s.Id,
                Name = s.Name,
                Marks = worksList.Select(work => new MarkEntry
                {
                    WorkId = work.Id,
                    Name = work.Name!,
                    Value = s.MarksByWorkId[work.Id].FirstOrDefault()?.Value ?? "",
                    IsOverdue = s.MarksByWorkId[work.Id].FirstOrDefault()?.IsOverdue ?? false
                }).ToList()
            }).ToList();

            return result;
        }

        public async Task<bool> UpdateStudentAsync(Student entity, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            context.Update(entity);
            await context.SaveChangesAsync(cancellationToken: cancellationToken);
            return true;
        }

        public async Task<List<Student>> GetStudentsByIdsAsync(IEnumerable<int> studentIds, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            return await context.Students
                .AsNoTracking()
                .Where(s => studentIds.Contains(s.Id))
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<List<Student>> GetStudentsByGroupIdsAsync(IEnumerable<int> groupIds, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var entities = await context.Students
                .AsNoTracking()
                .Where(s => groupIds.Contains(s.GroupId))
                .ToListAsync(cancellationToken: cancellationToken);
            return entities;
        }

        public async Task BulkInsertAsync(IEnumerable<Student> students, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var bulkConfig = new BulkConfig { UpdateByProperties = [nameof(Student.OfficialId), nameof(Student.GroupId)] };
            await context.BulkInsertOrUpdateAsync(students, bulkConfig, cancellationToken: cancellationToken);
        }



        public async Task DeleteAllAsync(CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            await context.Students.ExecuteDeleteAsync(cancellationToken: cancellationToken);
        }

        public async Task<List<Student>> GetArchivedByGroupIdsAsync(IEnumerable<int> groupIds, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var archivedStudents = await context.ReportSnapshots
                .AsNoTracking()
                .Where(r => groupIds.Contains(r.GroupId))
                .Select(r => new
                {
                    r.StudentId,
                    r.StudentName
                })
                .Distinct()
                .Select(r => new Student { Id = r.StudentId, Name = r.StudentName })
                .ToListAsync(cancellationToken: cancellationToken);
            return archivedStudents;
        }

        public async Task DeleteByIdsAsync(IEnumerable<int> studentsOfficialIds, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            await context.Students.Where(s => studentsOfficialIds.Contains(s.OfficialId))
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}