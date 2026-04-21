using BgituGrades.Application.Models.Transfer;
using BgituGrades.Domain.Interfaces;
using FluentValidation;

namespace BgituGrades.Application.Validators
{
    public class CreateTransferRequestValidator : AbstractValidator<CreateTransferRequest>
    {
        public CreateTransferRequestValidator(IGroupRepository groupRepository, IDisciplineRepository disciplineRepository)
        {
            RuleFor(x => x.DisciplineId)
                .MustAsync(async (disciplineId, cancellationToken) => await disciplineRepository.ExistsAsync(disciplineId, cancellationToken))
                .WithMessage((x) => $"DisciplineId = {x.DisciplineId} не существует");

            RuleFor(x => x.GroupId)
                .MustAsync(async (groupId, cancellationToken) => await groupRepository.ExistsAsync(groupId, cancellationToken))
                .WithMessage((x) => $"GroupId = {x.GroupId} не существует");

            RuleFor(x => x.OriginalDate)
                .NotEmpty().WithMessage("Исходная дата не может быть пустой");

            RuleFor(x => x.NewDate)
                .NotEmpty().WithMessage("Новая дата не может быть пустой")
                .NotEqual(x => x.OriginalDate).WithMessage("Новая дата должна отличаться от исходной");
        }
    }

    public class UpdateTransferRequestValidator : AbstractValidator<UpdateTransferRequest>
    {
        public UpdateTransferRequestValidator(ITransferRepository transferRepository)
        {
            RuleFor(x => x.Id)
                .MustAsync(async (id, cancellationToken) => await transferRepository.ExistsAsync(id, cancellationToken))
                .WithMessage((x) => $"Id = {x.Id} не существует");
            RuleFor(x => x.NewDate)
                .NotEmpty().WithMessage("Новая дата не может быть пустой");
        }
    }
}
