using BgituGrades.Application.Models.Class;
using BgituGrades.Domain.Interfaces;
using FluentValidation;

namespace BgituGrades.Application.Validators
{
    public class CreateClassRequestValidator : AbstractValidator<CreateClassRequest>
    {
        public CreateClassRequestValidator(IDisciplineRepository disciplineRepository, IGroupRepository groupRepository)
        {
            RuleFor(x => x.WeekDay)
                .InclusiveBetween(1, 7)
                .WithMessage("День недели должен быть от 1 до 7");

            RuleFor(x => x.Weeknumber)
                .InclusiveBetween(1, 2)
                .WithMessage("Номер недели должен быть 1 или 2");

            RuleFor(x => x.DisciplineId)
                .MustAsync(async (disciplineId, cancellationToken) => await disciplineRepository.ExistsAsync(disciplineId, cancellationToken))
                .WithMessage((x) => $"DisciplineId = {x.DisciplineId} не существует");

            RuleFor(x => x.GroupId)
                .MustAsync(async (groupId, cancellationToken) => await groupRepository.ExistsAsync(groupId, cancellationToken))
                .WithMessage((x) => $"GroupId = {x.GroupId} не существует");
        }
    }

    public class GetClassDateRequestValidator : AbstractValidator<GetClassDateRequest>
    {
        public GetClassDateRequestValidator(IDisciplineRepository disciplineRepository, IGroupRepository groupRepository)
        {
            RuleFor(x => x.GroupId)
                .MustAsync(async (groupId, cancellationToken) => await groupRepository.ExistsAsync(groupId, cancellationToken))
                .WithMessage((x) => $"GroupId = {x.GroupId} не существует");

            RuleFor(x => x.DisciplineId)
                .MustAsync(async (disciplineId, cancellationToken) => await disciplineRepository.ExistsAsync(disciplineId, cancellationToken))
                .WithMessage((x) => $"DisciplineId = {x.DisciplineId} не существует");
        }
    }
}
