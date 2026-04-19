using BgituGrades.Application.Models.Presence;
using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Interfaces;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

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
    }
}
