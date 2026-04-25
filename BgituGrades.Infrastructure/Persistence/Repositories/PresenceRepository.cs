using BgituGrades.Application.Models.Presence;
using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Enums;
using BgituGrades.Domain.Interfaces;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BgituGrades.Infrastructure.Persistence.Repositories
{
    

    public class PresenceRepository(IDbContextFactory<AppDbContext> contextFactory) : IPresenceRepository
    {
        public async Task<Presence> CreatePresenceAsync(Presence entity, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            await context.Presences.AddAsync(entity, cancellationToken: cancellationToken);
            await context.SaveChangesAsync(cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<List<Presence>> GetAllPresencesAsync(CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var entities = await context.Presences
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
            return entities;
        }

        public async Task<List<Presence>> GetPresencesByDisciplineAndGroupAsync(int disciplineId, int groupId, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var entities = await context.Presences
                .Where(p => p.DisciplineId == disciplineId &&
                           p.Student!.GroupId == groupId)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
            return entities;
        }

        public async Task<bool> DeletePresenceByStudentAndDateAsync(int studentId, DateOnly date, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var result = await context.Presences
                .Where(p => p.StudentId == studentId && p.Date == date)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
            return result > 0;
        }

        public async Task UpdatePresenceAsync(Presence entity, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            context.Presences.Update(entity);
            await context.SaveChangesAsync(cancellationToken: cancellationToken);
        }

        public async Task DeleteAllAsync(CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            await context.Presences.ExecuteDeleteAsync(cancellationToken: cancellationToken);
        }

        public async Task<Presence?> GetAsync(Presence entity, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var presence = await context.Presences
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.DisciplineId == entity.DisciplineId &&
                                          p.StudentId == entity.StudentId &&
                                          p.ClassId == entity.ClassId &&
                                          p.Date == entity.Date, cancellationToken: cancellationToken);
            return presence;
        }

        public async Task<List<Presence>> GetPresencesByDisciplinesAndGroupsAsync(IEnumerable<int> disciplineIds, IEnumerable<int> groupIds, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var entities = await context.Presences
                .Where(p => disciplineIds.Contains(p.DisciplineId) &&
                           groupIds.Contains(p.Student!.GroupId))
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
            return entities;
        }

        public async Task<Presence?> GetPresenceByIdAsync(int id, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            return await context.Presences
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken: cancellationToken);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            return await context.Presences.AnyAsync(p => p.Id == id, cancellationToken: cancellationToken);
        }


        public async Task<(int present, int total)?> GetPresenceCountAsync(
            string groupName, string disciplineName,
            DateOnly date, TimeOnly startTime,
            CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);

            int weekDay = (int)date.DayOfWeek;
            int weekNumber = ISOWeek.GetWeekOfYear(date.ToDateTime(TimeOnly.MinValue)) % 2 == 0 ? 2 : 1;

            var classId = await context.Classes
                .AsNoTracking()
                .Where(c =>
                    c.Group!.Name!.ToLower() == groupName.ToLower() &&
                    c.Discipline!.Name!.ToLower() == disciplineName.ToLower() &&
                    c.WeekDay == weekDay &&
                    c.Weeknumber == weekNumber &&
                    c.StartTime.Hour == startTime.Hour &&
                    c.StartTime.Minute == startTime.Minute)
                .Select(c => (int?)c.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (classId == null)
                return null;

            var total = await context.Students
                .AsNoTracking()
                .CountAsync(s => s.Group!.Name!.ToLower() == groupName.ToLower(), cancellationToken);

            var absentCount = await context.Presences
                .AsNoTracking()
                .CountAsync(p => p.ClassId == classId && p.Date == date && 
                    (p.IsPresent == PresenceType.ABSENTVALID || p.IsPresent == PresenceType.ABSENTINVALID), 
                    cancellationToken);

            return (total - absentCount, total);
        }

        public async Task<(int present, int total, string GroupKey)?> GetPresenceCountByClassAsync(int classId, DateOnly date, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var classEntity = await context.Classes
                .Where(c => c.Id == classId)
                .Select(c => new
                {
                    GroupName = c.Group!.Name,
                    DisciplineName = c.Discipline!.Name,
                    c.StartTime
                })
                .FirstOrDefaultAsync(cancellationToken);
            
            if (classEntity == null)    
                return null;

            var total = await context.Students
                .CountAsync(s => s.Group!.Name == classEntity.GroupName, cancellationToken);

            var absentCount = await context.Presences
                .CountAsync(p => p.ClassId == classId && p.Date == date &&
                    (p.IsPresent == PresenceType.ABSENTVALID || p.IsPresent == PresenceType.ABSENTINVALID),
                    cancellationToken);

            var groupKey = $"count_{classEntity.GroupName}_{classEntity.DisciplineName}_{date:yyyy-MM-dd}_{classEntity.StartTime:HH-mm}";

            return (total - absentCount, total, groupKey);
        }
    }
}
