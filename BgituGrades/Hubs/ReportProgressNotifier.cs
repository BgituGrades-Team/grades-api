using BgituGrades.Application.Interfaces;
using BgituGrades.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace BgituGrades.API.Hubs
{
    public class ReportProgressNotifier(IHubContext<ReportHub> hubContext) : IReportProgressNotifier
    {
        public Task AddToGroupAsync(string connectionId, Guid reportId) =>
            hubContext.Groups.AddToGroupAsync(connectionId, reportId.ToString());   
        public Task SendProgressAsync(Guid reportId, int percent, string message) =>
            hubContext.Clients.Group(reportId.ToString())
                .SendAsync("ReportProgress", reportId.ToString(), percent, message);

        public Task SendReadyAsync(Guid reportId, string downloadUrl, object preview) =>
            hubContext.Clients.Group(reportId.ToString())
                .SendAsync("ReportReady", reportId.ToString(), downloadUrl, preview);

        public Task SendErrorAsync(Guid reportId, string message, string? stackTrace) =>
            hubContext.Clients.Group(reportId.ToString())
                .SendAsync("Error", message, stackTrace);
    }
}
