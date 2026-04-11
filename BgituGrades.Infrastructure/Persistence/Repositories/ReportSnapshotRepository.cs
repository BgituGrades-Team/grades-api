using BgituGrades.Data;
using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BgituGrades.Infrastructure.Persistence.Repositories
{
    

    public class ReportSnapshotRepository(IDbContextFactory<AppDbContext> contextFactory) : IReportSnapshotRepository
    {
        public async Task<bool> DeleteReportSnapshotAsync(int id, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var result = await context.ReportSnapshots
                .Where(t => t.Id == id)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
            return result > 0;
        }

        public async Task<ReportSnapshot?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var entity = await context.ReportSnapshots.FindAsync([id], cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<List<ReportSnapshot>> GetAllReportSnapshotsAsync(CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var entities = await context.ReportSnapshots
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
            return entities;
        }

        public async Task<List<ReportSnapshot>> GetReportSnapshotsByGroupAndDisciplineAsync(int groupId, int disciplineId, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var entities = await context.ReportSnapshots
                .Where(t => t.DisciplineId == disciplineId &&
                            t.GroupId == groupId)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
            return entities;
        }

        public async Task<Dictionary<(int GroupId, int DisciplineId), List<ReportSnapshot>>> GetReportSnapshotsByGroupIdsAsync(
            IEnumerable<int> groupIds, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var ReportSnapshots = await context.ReportSnapshots
                .Where(t => groupIds.Contains(t.GroupId))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return ReportSnapshots
                .GroupBy(t => (t.GroupId, t.DisciplineId))
                .ToDictionary(
                    g => g.Key,
                    g => g.ToList());
        }

        public async Task<List<ReportSnapshot>> GetReportSnapshotsByYearAndSemesterAsync(int year, int semester, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.ReportSnapshots
                .Where(s => s.Year == year && s.Semester == semester)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
