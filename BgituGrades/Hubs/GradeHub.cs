using BgituGrades.Models.Class;
using BgituGrades.Models.Mark;
using BgituGrades.Models.Presence;
using BgituGrades.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Saunter.Attributes;

namespace BgituGrades.Hubs
{
    [AsyncApi]
    public class GradeHub(IClassService classService, IPresenceService presenceService, IMarkService markService) : Hub
    {
        private readonly IClassService _classService = classService;
        private readonly IPresenceService _presenceService = presenceService;
        private readonly IMarkService _markService = markService;

        [Channel("hubs/grade/GetMarkGrade")]
        [Authorize(Policy = "ViewOnly")]
        [PublishOperation(typeof(GetClassDateRequest), Summary = "Запросить оценки по работам", OperationId = nameof(GetMarkGrade))]
        [SubscribeOperation(typeof(IEnumerable<FullGradeMarkResponse>), Summary = "Событие: Получение списка оценок (ответ на GetMarkGrade)", OperationId = "ReceiveMarks")]
        public async Task GetMarkGrade(GetClassDateRequest request, CancellationToken cancellationToken)
        {
            var marks = await _classService.GetMarksByWorksAsync(request, cancellationToken);
            await Clients.Caller.SendAsync("ReceiveMarks", marks, cancellationToken: cancellationToken);
        }

        [Channel("hubs/grade/GetPresenceGrade")]
        [Authorize(Policy = "ViewOnly")]
        [PublishOperation(typeof(GetClassDateRequest), Summary = "Запросить данные о посещаемости", OperationId = nameof(GetPresenceGrade))]
        [SubscribeOperation(typeof(IEnumerable<FullGradePresenceResponse>), Summary = "Событие: Получение данных о посещаемости (ответ на GetPresenceGrade)", OperationId = "ReceivePresences")]
        public async Task GetPresenceGrade(GetClassDateRequest request, CancellationToken cancellationToken)
        {
            var classDates = await _classService.GetPresenceByScheduleAsync(request, cancellationToken);
            await Clients.Caller.SendAsync("ReceivePresences", classDates, cancellationToken: cancellationToken);
        }

        [Channel("hubs/grade/UpdateMarkGrade")]
        [Authorize(Policy = "Edit")]
        [PublishOperation(typeof(UpdateMarkGradeRequest), Summary = "Обновить или создать оценку", OperationId = nameof(UpdateMarkGrade))]
        [SubscribeOperation(typeof(FullGradeMarkResponse), Summary = "Событие: Оценка обновлена (рассылается всем)", OperationId = "UpdatedMark")]
        public async Task UpdateMarkGrade(UpdateMarkGradeRequest request, CancellationToken cancellationToken)
        {
            var response = await _markService.UpdateOrCreateMarkAsync(request, cancellationToken: cancellationToken);
            await Clients.All.SendAsync("UpdatedMark", response, cancellationToken: cancellationToken);
        }

        [Channel("hubs/grade/UpdatePresenceGrade")]
        [Authorize(Policy = "Edit")]
        [PublishOperation(typeof(UpdatePresenceGradeRequest), Summary = "Обновить или создать запись о посещаемости", OperationId = nameof(UpdatePresenceGrade))]
        [SubscribeOperation(typeof(FullGradePresenceResponse), Summary = "Событие: Посещаемость обновлена (рассылается всем)", OperationId = "UpdatedPresence")]
        public async Task UpdatePresenceGrade(UpdatePresenceGradeRequest request, CancellationToken cancellationToken)
        {
            var response = await _presenceService.UpdateOrCreatePresenceAsync(request, cancellationToken: cancellationToken);
            await Clients.All.SendAsync("UpdatedPresence", response, cancellationToken: cancellationToken);
        }
    }
}
