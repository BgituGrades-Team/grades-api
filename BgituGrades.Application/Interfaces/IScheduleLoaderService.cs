namespace BgituGrades.Application.Interfaces
{
    public interface IScheduleLoaderService
    {
        Task<bool> RunAsync(string apiKey, CancellationToken cancellationToken);
    }
}
