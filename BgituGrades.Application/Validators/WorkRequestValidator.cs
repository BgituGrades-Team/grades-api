using BgituGrades.Application.Models.Work;
using BgituGrades.Domain.Interfaces;
using FluentValidation;

namespace BgituGrades.Application.Validators
{
    public class CreateWorkRequestValidator : AbstractValidator<CreateWorkRequest>
    {
        public CreateWorkRequestValidator(IDisciplineRepository disciplineRepository)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Имя работы не может быть пустым")
                .MaximumLength(255).WithMessage("Имя работы не может быть длиннее 255 символов");

            RuleFor(x => x.DisciplineId)
                .MustAsync(async (disciplineId, cancellationToken) => await disciplineRepository.ExistsAsync(disciplineId, cancellationToken))
                .WithMessage((x) => $"DisciplineId = {x.DisciplineId} не существует");
        }
    }

    public class UpdateWorkRequestValidator : AbstractValidator<UpdateWorkRequest>
    {
        public UpdateWorkRequestValidator(IWorkRepository workRepository, IDisciplineRepository disciplineRepository)
        {
            RuleFor(x => x.Id)
                .MustAsync(async (id, cancellationToken) => await workRepository.ExistsAsync(id, cancellationToken))
                .WithMessage((x) => $"Id = {x.Id} не существует");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Имя работы не может быть пустым")
                .MaximumLength(255).WithMessage("Имя работы не может быть длиннее 255 символов");

            RuleFor(x => x.DisciplineId)
                .MustAsync(async (disciplineId, cancellationToken) => await disciplineRepository.ExistsAsync(disciplineId, cancellationToken))
                .WithMessage((x) => $"DisciplineId = {x.DisciplineId} не существует");

            RuleFor(x => x.IssuedDate)
                .NotEmpty().WithMessage("Дата выдачи не может быть пустой");
        }
    }
}
