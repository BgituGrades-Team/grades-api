using BgituGrades.Application.Models.Discipline;
using BgituGrades.Domain.Interfaces;
using FluentValidation;

namespace BgituGrades.Application.Validators
{
    public class CreateDisciplineRequestValidator : AbstractValidator<CreateDisciplineRequest>
    {
        public CreateDisciplineRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Имя дисциплины не может быть пустым")
                .MaximumLength(255).WithMessage("Имя дисциплины не может быть длиннее 255 символов");
        }
    }

    public class GetDisciplineRequestValidator : AbstractValidator<GetDisciplineByGroupIdsRequest>
    {
        public GetDisciplineRequestValidator()
        {
            RuleFor(x => x.GroupIds)
                .NotEmpty().WithMessage("groupIds не может быть пустым");
        }
    }

    public class UpdateDisciplineRequestValidator : AbstractValidator<UpdateDisciplineRequest>
    {
        public UpdateDisciplineRequestValidator(IDisciplineRepository disciplineRepository)
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .MustAsync(async (id, cancellationToken) => await disciplineRepository.ExistsAsync(id, cancellationToken))
                .WithMessage((x) => $"Id = {x.Id} не существует");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Имя дисциплины не может быть пустым")
                .MaximumLength(255).WithMessage("Имя дисциплины не может быть длиннее 255 символов");
        }
    }

    public class DeleteDisciplineRequestValidator : AbstractValidator<DeleteDisciplineRequest>
    {
        public DeleteDisciplineRequestValidator(IDisciplineRepository disciplineRepository)
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .MustAsync(async (id, cancellationToken) => await disciplineRepository.ExistsAsync(id, cancellationToken))
                .WithMessage((x) => $"Id = {x.Id} не существует");
        }
    }
}
