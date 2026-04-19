using BgituGrades.Application.Models.Group;
using BgituGrades.Domain.Interfaces;
using FluentValidation;

namespace BgituGrades.Application.Validators
{
    public class CreateGroupRequestValidator : AbstractValidator<CreateGroupRequest>
    {
        public CreateGroupRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                    .WithMessage("Имя группы не может быть пустым")
                .MaximumLength(255)
                    .WithMessage("Имя группы не может быть длиннее 255 символов");

            RuleFor(x => x.StudyStartDate)
                .LessThan(x => x.StudyEndDate)
                    .WithMessage("Дата начала должна быть раньше даты окончания");

            RuleFor(x => x.StartWeekNumber)
                .InclusiveBetween(1, 2)
                .WithMessage("Номер недели начала обучения должен быть 1 или 2");
        }
    }
    public class UpdateGroupRequestValidator : AbstractValidator<UpdateGroupRequest>
    {
        public UpdateGroupRequestValidator(IGroupRepository groupRepository)
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .MustAsync(async (id, cancellationToken) => await groupRepository.ExistsAsync(id, cancellationToken))
                .WithMessage((x) => $"Id = {x.Id} не существует");

            RuleFor(x => x.Name)
                .NotEmpty()
                    .WithMessage("Имя группы не может быть пустым")
                .MaximumLength(255)
                    .WithMessage("Имя группы не может быть длиннее 255 символов");

            RuleFor(x => x.StudyStartDate)
                .LessThan(x => x.StudyEndDate)
                    .WithMessage("Дата начала должна быть раньше даты окончания");

            RuleFor(x => x.StartWeekNumber)
                .InclusiveBetween(1, 2)
                .WithMessage("Номер недели начала обучения должен быть 1 или 2");
        }
    }

    public class GetGroupsByDisciplineRequestValidator : AbstractValidator<GetGroupsByDisciplineRequest>
    {
        public GetGroupsByDisciplineRequestValidator(IDisciplineRepository disciplineRepository)
        {
            RuleFor(x => x.DisciplineId)
                .GreaterThan(0)
                .MustAsync(async (disciplineId, cancellationToken) => await disciplineRepository.ExistsAsync(disciplineId, cancellationToken))
                .WithMessage((x) => $"DisciplineId = {x.DisciplineId} не существует");
        }
    }
}
