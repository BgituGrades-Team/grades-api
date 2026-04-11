using BgituGrades.Application.Models.Report;

namespace BgituGrades.Application.Interfaces
{
    public interface IMigrationService
    {
        Task DeleteAll(CancellationToken cancellationToken);
        Task ArchiveCurrentSemesterAsync(CancellationToken cancellationToken);
        Task<List<PeriodResponse>> GetAllPeriods(CancellationToken cancellationToken);
        protected static int GetCurrentSemester(DateOnly date) =>
                date.Month >= 9 ? 1 : 2;
        protected static int GetCurrentSemester() =>
                GetCurrentSemester(DateOnly.FromDateTime(DateTime.Now));
    }
}
