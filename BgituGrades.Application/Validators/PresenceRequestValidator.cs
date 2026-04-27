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
                .MustAsync(async (studentId, cancellationToken) => await studentRepository.ExistsAsync(studentId, cancellationToken))
                .WithMessage((x) => $"StudentId = {x.StudentId} не существует");

            RuleFor(x => x.DisciplineId)
                .MustAsync(async (disciplineId, cancellationToken) => await disciplineRepository.ExistsAsync(disciplineId, cancellationToken))
                .WithMessage((x) => $"DisciplineId = {x.DisciplineId} не существует");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Дата не может быть пустой");
        }
    }

    public class GetPresenceCountRequestValidator : AbstractValidator<GetPresenceCountRequest>
    {
        public GetPresenceCountRequestValidator()
        {
            RuleFor(x => x.GroupName)
                .NotEmpty().WithMessage("GroupName не может быть пустым");

            RuleFor(x => x.DisciplineName)
                .NotEmpty().WithMessage("DisciplineName не может быть пустым");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Дата не может быть пустой");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Время начала не может быть пустым");
        }
    }

    public class UpdatePresenceRequestValidator : AbstractValidator<UpdatePresenceRequest>
    {
        public UpdatePresenceRequestValidator(IPresenceRepository presenceRepository, IStudentRepository studentRepository, IDisciplineRepository disciplineRepository)
        {
            RuleFor(x => x.Id)
                .MustAsync(async (id, cancellationToken) => await presenceRepository.ExistsAsync(id, cancellationToken))
                .WithMessage((x) => $"Id = {x.Id} не существует");

            RuleFor(x => x.StudentId)
                .MustAsync(async (studentId, cancellationToken) => await studentRepository.ExistsAsync(studentId, cancellationToken))
                .WithMessage((x) => $"StudentId = {x.StudentId} не существует");

            RuleFor(x => x.DisciplineId)
                .MustAsync(async (disciplineId, cancellationToken) => await disciplineRepository.ExistsAsync(disciplineId, cancellationToken))
                .WithMessage((x) => $"DisciplineId = {x.DisciplineId} не существует");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Дата не может быть пустой");
        }
    }

    public class UpdatePresenceGradeRequestValidator : AbstractValidator<UpdatePresenceGradeRequest>
    {
        public UpdatePresenceGradeRequestValidator(IStudentRepository studentRepository, IDisciplineRepository disciplineRepository, IClassRepository classRepository)
        {
            RuleFor(x => x.ClassId)
                .MustAsync(async (id, cancellationToken) => await classRepository.ExistsAsync(id, cancellationToken))
                .WithMessage((x) => $"ClassId = {x.ClassId} не существует");
            RuleFor(x => x.IsPresent)
                .IsInEnum()
                .WithMessage("Недопустимое значение присутствия");
            RuleFor(x => x.StudentId)
                .MustAsync(async (studentId, cancellationToken) => await studentRepository.ExistsAsync(studentId, cancellationToken))
                .WithMessage((x) => $"StudentId = {x.StudentId} не существует");

            RuleFor(x => x.DisciplineId)
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
                .MustAsync(async (disciplineId, cancellationToken) => await disciplineRepository.ExistsAsync(disciplineId, cancellationToken))
                .WithMessage((x) => $"DisciplineId = {x.DisciplineId} не существует");

            RuleFor(x => x.GroupId)
                .MustAsync(async (groupId, cancellationToken) => await groupRepository.ExistsAsync(groupId, cancellationToken))
                .WithMessage((x) => $"GroupId = {x.GroupId} не существует");
        }
    }

    public class DeletePresenceByStudentAndDateRequestValidator : AbstractValidator<DeletePresenceByStudentAndDateRequest>
    {
        public DeletePresenceByStudentAndDateRequestValidator(IStudentRepository studentRepository)
        {
            RuleFor(x => x.StudentId)
                .MustAsync(async (studentId, cancellationToken) => await studentRepository.ExistsAsync(studentId, cancellationToken))
                .WithMessage((x) => $"StudentId = {x.StudentId} не существует");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Дата не может быть пустой");
        }
    }
}
