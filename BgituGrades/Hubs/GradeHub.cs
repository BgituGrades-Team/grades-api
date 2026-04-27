using BgituGrades.Application.Interfaces;
using BgituGrades.Application.Models.Class;
using BgituGrades.Application.Models.Mark;
using BgituGrades.Application.Models.Presence;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Saunter.Attributes;

namespace BgituGrades.API.Hubs
{
    [AsyncApi]
    public class GradeHub(IClassService classService, IPresenceService presenceService, IMarkService markService, IServiceProvider serviceProvider) : Hub
    {
        private readonly IClassService _classService = classService;
        private readonly IPresenceService _presenceService = presenceService;
        private readonly IMarkService _markService = markService;

        [Channel("hubs/grade/GetMarkGrade")]
        [Authorize(Policy = "ViewOnly")]
        [PublishOperation(typeof(GetClassDateRequest), Summary = "Запросить оценки по работам", OperationId = nameof(GetMarkGrade))]
        [SubscribeOperation(typeof(List<FullGradeMarkResponse>), Summary = "Событие: Получение списка оценок (ответ на GetMarkGrade)", OperationId = "ReceiveMarks")]
        public async Task GetMarkGrade(GetClassDateRequest request)
        {
            var groupClaim = Context.User?.FindFirst("group_id")?.Value;
            if (Context.User?.IsInRole("STUDENT") == true)
            {
                if (groupClaim == null || groupClaim != request.GroupId.ToString())
                {
                    await Clients.Caller.SendAsync("PermissionDenied", "Доступ запрещён");
                    return;
                }
            }
            var validator = serviceProvider.GetService<IValidator<GetClassDateRequest>>();
            var validationResult = validator != null ? await validator.ValidateAsync(request) : null;
            if (validationResult == null || !validationResult.IsValid)
            {
                var errors = validationResult?.Errors.Select(e => e.ErrorMessage).ToList() ?? ["Validation failed"];
                await Clients.Caller.SendAsync("ValidationError", errors);
                return;
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, $"class_{request.GroupId}_{request.DisciplineId}");
            var cancellationToken = Context.ConnectionAborted;
            var marks = await _classService.GetMarksByWorksAsync(request.GroupId, request.DisciplineId, cancellationToken);
            await Clients.Caller.SendAsync("ReceiveMarks", marks);
        }

        [Channel("hubs/grade/GetPresenceGrade")]
        [Authorize(Policy = "ViewOnly")]
        [PublishOperation(typeof(GetClassDateRequest), Summary = "Запросить данные о посещаемости", OperationId = nameof(GetPresenceGrade))]
        [SubscribeOperation(typeof(List<FullGradePresenceResponse>), Summary = "Событие: Получение данных о посещаемости (ответ на GetPresenceGrade)", OperationId = "ReceivePresences")]
        public async Task GetPresenceGrade(GetClassDateRequest request)
        {
            var groupClaim = Context.User?.FindFirst("group_id")?.Value;
            if (Context.User?.IsInRole("STUDENT") == true)
            {
                if (groupClaim == null || groupClaim != request.GroupId.ToString())
                {
                    await Clients.Caller.SendAsync("PermissionDenied", "Доступ запрещён");
                    return;
                }
            }
            var validator = serviceProvider.GetService<IValidator<GetClassDateRequest>>();
            var validationResult = validator != null ? await validator.ValidateAsync(request) : null;
            if (validationResult == null || !validationResult.IsValid)
            {
                var errors = validationResult?.Errors.Select(e => e.ErrorMessage).ToList() ?? ["Validation failed"];
                await Clients.Caller.SendAsync("ValidationError", errors);
                return;
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, $"class_{request.GroupId}_{request.DisciplineId}");
            var cancellationToken = Context.ConnectionAborted;
            var classDates = await _classService.GetPresenceByScheduleAsync(request.GroupId, request.DisciplineId, cancellationToken);
            await Clients.Caller.SendAsync("ReceivePresences", classDates);
        }

        [Channel("hubs/grade/UpdateMarkGrade")]
        [Authorize(Policy = "Edit")]
        [PublishOperation(typeof(UpdateMarkGradeRequest), Summary = "Обновить или создать оценку", OperationId = nameof(UpdateMarkGrade))]
        [SubscribeOperation(typeof(FullGradeMarkResponse), Summary = "Событие: Оценка обновлена (рассылается всем)", OperationId = "UpdatedMark")]
        public async Task UpdateMarkGrade(UpdateMarkGradeRequest request)
        {
            var validator = serviceProvider.GetService<IValidator<UpdateMarkGradeRequest>>();
            var validationResult = validator != null ? await validator.ValidateAsync(request) : null;
            if (validationResult == null || !validationResult.IsValid)
            {
                var errors = validationResult?.Errors.Select(e => e.ErrorMessage).ToList() ?? ["Validation failed"];
                await Clients.Caller.SendAsync("ValidationError", errors);
                return;
            }
            var cancellationToken = Context.ConnectionAborted;
            var response = await _markService.UpdateOrCreateMarkAsync(request, cancellationToken: cancellationToken);
            await Clients.Groups($"class_{request.GroupId}_{request.DisciplineId}").SendAsync("UpdatedMark", response);
        }

        [Channel("hubs/grade/UpdatePresenceGrade")]
        [Authorize(Policy = "Edit")]
        [PublishOperation(typeof(UpdatePresenceGradeRequest), Summary = "Обновить или создать запись о посещаемости", OperationId = nameof(UpdatePresenceGrade))]
        [SubscribeOperation(typeof(FullGradePresenceResponse), Summary = "Событие: Посещаемость обновлена (рассылается всем)", OperationId = "UpdatedPresence")]
        public async Task UpdatePresenceGrade(UpdatePresenceGradeRequest request)
        {
            var validator = serviceProvider.GetService<IValidator<UpdatePresenceGradeRequest>>();
            var validationResult = validator != null ? await validator.ValidateAsync(request) : null;
            if (validationResult == null || !validationResult.IsValid)
            {
                var errors = validationResult?.Errors.Select(e => e.ErrorMessage).ToList() ?? ["Validation failed"];
                await Clients.Caller.SendAsync("ValidationError", errors);
                return;
            }
            var cancellationToken = Context.ConnectionAborted;
            var response = await _presenceService.UpdateOrCreatePresenceAsync(request, cancellationToken: cancellationToken);
            var result = await _presenceService.GetPresenceCountByClassAsync(request.ClassId, request.Date, cancellationToken);
            var countResponse = new PresenceCountResponse
            {
                Present = result?.present ?? 0,
                Total = result?.total ?? 0,
                DisciplineName = result?.DisciplineName ?? string.Empty,
                StartTime = result?.StartTime ?? default,
                Date = result?.Date ?? default
            };
            if (result != null)
            {
                await Clients.Groups(result.Value.GroupKey).SendAsync("ReceivePresenceCount", countResponse);
            }
            await Clients.Groups($"class_{request.GroupId}_{request.DisciplineId}").SendAsync("UpdatedPresence", response);
        }

        [Channel("hubs/grade/GetPresenceCount")]
        [AllowAnonymous]
        [PublishOperation(typeof(GetPresenceCountRequest), Summary = "Подписаться на счётчик присутствующих", OperationId = nameof(GetPresenceCount))]
        [SubscribeOperation(typeof(PresenceCountResponse), Summary = "Событие: Текущий счётчик присутствующих", OperationId = "ReceivePresenceCount")]
        public async Task GetPresenceCount(GetPresenceCountRequest request)
        {
            var validator = serviceProvider.GetService<IValidator<GetPresenceCountRequest>>();
            var validationResult = validator != null ? await validator.ValidateAsync(request) : null;
            if (validationResult == null || !validationResult.IsValid)
            {
                var errors = validationResult?.Errors.Select(e => e.ErrorMessage).ToList() ?? ["Validation failed"];
                await Clients.Caller.SendAsync("ValidationError", errors);
                return;
            }

            var cancellationToken = Context.ConnectionAborted;
            var result = await _presenceService.GetPresenceCountAsync(
                request.GroupName, request.DisciplineName,
                request.Date, request.StartTime, cancellationToken);
            if (result == null)
            {
                await Clients.Caller.SendAsync("NotFound", "Пара или записи о посещаемости не найдены");
                return;
            }
            var response = new PresenceCountResponse
            {
                Present = result.Value.present,
                Total = result.Value.total,
                DisciplineName = result.Value.DisciplineName,
                StartTime = result.Value.StartTime,
                Date = result.Value.Date
            };
            var groupKey = $"count_{request.GroupName.ToLower()}_{request.DisciplineName.ToLower()}_{request.Date:yyyy-MM-dd}_{request.StartTime:HH-mm}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupKey);
            await Clients.Caller.SendAsync("ReceivePresenceCount", response);
        }

        public record PermissionDeniedResponse(string Message);

        [Channel("hubs/grade/PermissionDenied")]
        [SubscribeOperation(typeof(PermissionDeniedResponse), Summary = "Событие: Доступ запрещён, т.к. groupId не соответствует запрашиваемому", OperationId = "PermissionDenied")]
        public void PermissionDenied(PermissionDeniedResponse response) { }
    }
}
