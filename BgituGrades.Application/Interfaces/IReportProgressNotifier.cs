namespace BgituGrades.Application.Interfaces
{
    public interface IReportProgressNotifier
    {
        Task AddToGroupAsync(string connectionId, Guid reportId);
        Task SendProgressAsync(Guid reportId, int percent, string message);
        Task SendReadyAsync(Guid reportId, string downloadUrl, object preview);
        Task SendErrorAsync(Guid reportId, string message, string? stackTrace);
    }
}
