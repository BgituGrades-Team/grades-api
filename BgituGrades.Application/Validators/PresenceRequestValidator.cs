using BgituGrades.Application.Models.Presence;
using BgituGrades.Domain.Interfaces;
using FluentValidation;

namespace BgituGrades.Application.Validators
{
    public class CreatePresenceRequestValidator : AbstractValidator<CreatePresenceRequest>
    {
        public CreatePresenceRequestValidator(IStudentRepository studentRepository, IDisciplineRepository disciplineRepository)
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0)
                .MustAsync(async (studentId, cancellationToken) => await studentRepository.ExistsAsync(studentId, cancellationToken))
                .WithMessage((x) => $"StudentId = {x.StudentId} не существует");

            RuleFor(x => x.DisciplineId)
                .GreaterThan(0)
                .MustAsync(async (disciplineId, cancellationToken) => await disciplineRepository.ExistsAsync(disciplineId, cancellationToken))
                .WithMessage((x) => $"DisciplineId = {x.DisciplineId} не существует");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Дата не может быть пустой");
        }
    }

    public class UpdatePresenceRequestValidator : AbstractValidator<UpdatePresenceRequest>
    {
        public UpdatePresenceRequestValidator(IPresenceRepository presenceRepository, IStudentRepository studentRepository, IDisciplineRepository disciplineRepository)
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .MustAsync(async (id, cancellationToken) => await presenceRepository.ExistsAsync(id, cancellationToken))
                .WithMessage((x) => $"Id = {x.Id} не существует");

            RuleFor(x => x.StudentId)
                .GreaterThan(0)
                .MustAsync(async (studentId, cancellationToken) => await studentRepository.ExistsAsync(studentId, cancellationToken))
                .WithMessage((x) => $"StudentId = {x.StudentId} не существует");

            RuleFor(x => x.DisciplineId)
                .GreaterThan(0)
                .MustAsync(async (disciplineId, cancellationToken) => await disciplineRepository.ExistsAsync(disciplineId, cancellationToken))
                .WithMessage((x) => $"DisciplineId = {x.DisciplineId} не существует");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Дата не может быть пустой");
        }
    }

    public class GetPresenceByDisciplineAndGroupRequestValidator : AbstractValidator<GetPresenceByDisciplineAndGroupRequest>
    {
        public GetPresenceByDisciplineAndGroupRequestValidator(IDisciplineRepository disciplineRepository, IGroupRepository groupRepository)
        {
            RuleFor(x => x.DisciplineId)
                .GreaterThan(0)
                .MustAsync(async (disciplineId, cancellationToken) => await disciplineRepository.ExistsAsync(disciplineId, cancellationToken))
                .WithMessage((x) => $"DisciplineId = {x.DisciplineId} не существует");

            RuleFor(x => x.GroupId)
                .GreaterThan(0)
                .MustAsync(async (groupId, cancellationToken) => await groupRepository.ExistsAsync(groupId, cancellationToken))
                .WithMessage((x) => $"GroupId = {x.GroupId} не существует");
        }
    }

    public class DeletePresenceByStudentAndDateRequestValidator : AbstractValidator<DeletePresenceByStudentAndDateRequest>
    {
        public DeletePresenceByStudentAndDateRequestValidator(IStudentRepository studentRepository)
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0)
                .MustAsync(async (studentId, cancellationToken) => await studentRepository.ExistsAsync(studentId, cancellationToken))
                .WithMessage((x) => $"StudentId = {x.StudentId} не существует");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Дата не может быть пустой");
        }
    }
}
