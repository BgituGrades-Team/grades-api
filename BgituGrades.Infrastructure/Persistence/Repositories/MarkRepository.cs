using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BgituGrades.Infrastructure.Persistence.Repositories
{
    

    public class MarkRepository(IDbContextFactory<AppDbContext> contextFactory) : IMarkRepository
    {

        public async Task<Mark> CreateMarkAsync(Mark entity, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            await context.Marks.AddAsync(entity, cancellationToken: cancellationToken);
            await context.SaveChangesAsync(cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<List<Mark>> GetAllMarksAsync(CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var entities = await context.Marks
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
            return entities;
        }

        public async Task<List<Mark>> GetMarksByDisciplineAndGroupAsync(int disciplineId, int groupId, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var entities = await context.Marks
                .Where(m => m.Work!.DisciplineId == disciplineId &&
                            m.Student!.GroupId == groupId)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
            return entities;
        }



        public async Task<bool> UpdateMarkAsync(Mark entity, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            context.Update(entity);
            await context.SaveChangesAsync(cancellationToken: cancellationToken);
            return true;
        }

        public async Task<bool> DeleteMarkByStudentAndWorkAsync(int studentId, int workId, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var result = await context.Marks
                .Where(m => m.StudentId == studentId && m.WorkId == workId)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
            return result > 0;
        }

        public async Task DeleteAllAsync(CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            await context.Marks.ExecuteDeleteAsync(cancellationToken: cancellationToken);
        }

        public async Task<Mark?> GetMarkByStudentAndWorkAsync(int studentId, int workId, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            return await context.Marks
                .Where(m => m.StudentId == studentId && m.WorkId == workId)
                    .Include(m => m.Work)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }

        public async Task<double> GetAverageMarkByStudentAndDisciplineAsync(int studentId, int disciplineId, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var marks = await context.Marks
                .Where(m => m.StudentId == studentId && m.Work!.DisciplineId == disciplineId)
                .Select(m => m.Value)
                .ToListAsync(cancellationToken: cancellationToken);

            var validMarks = marks
                .Where(m => double.TryParse(m, out _))
                .Select(double.Parse!)
                .ToList();

            return validMarks.Count > 0 ? validMarks.Average() : 0;
        }

        public async Task<List<Mark>> GetMarksByDisciplinesAndGroupsAsync(IEnumerable<int> disciplineIds, IEnumerable<int> groupIds, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var entities = await context.Marks
                .Where(m => disciplineIds.Contains(m.Work!.DisciplineId) && groupIds.Contains(m.Student!.GroupId))
                    .Include(m => m.Work)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
            return entities;
        }

        public async Task<Mark?> GetMarkByIdAsync(int id, CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            return await context.Marks
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken: cancellationToken);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
