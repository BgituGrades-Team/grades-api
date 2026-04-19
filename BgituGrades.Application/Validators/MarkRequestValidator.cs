using BgituGrades.Application.Models.Mark;
using BgituGrades.Domain.Interfaces;
using FluentValidation;

namespace BgituGrades.Application.Validators
{
    public class CreateMarkRequestValidator : AbstractValidator<CreateMarkRequest>
    {
        public CreateMarkRequestValidator(IStudentRepository studentRepository, IWorkRepository workRepository)
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0)
                .MustAsync(async (studentId, cancellationToken) => await studentRepository.ExistsAsync(studentId, cancellationToken))
                .WithMessage((x) => $"StudentId = {x.StudentId} не существует");

            RuleFor(x => x.WorkId)
                .GreaterThan(0)
                .MustAsync(async (workId, cancellationToken) => await workRepository.ExistsAsync(workId, cancellationToken))
                .WithMessage((x) => $"WorkId = {x.WorkId} не существует");

            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("Оценка не может быть пустой")
                .MaximumLength(1).WithMessage("Оценка не может быть длиннее 1 символа");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Дата не может быть пустой");
        }
    }

    public class UpdateMarkRequestValidator : AbstractValidator<UpdateMarkRequest>
    {
        public UpdateMarkRequestValidator(IStudentRepository studentRepository, IWorkRepository workRepository, IMarkRepository markRepository)
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .MustAsync(async (id, cancellationToken) => await markRepository.ExistsAsync(id, cancellationToken))
                .WithMessage((x) => $"Id = {x.Id} не существует");

            RuleFor(x => x.StudentId)
                .GreaterThan(0)
                .MustAsync(async (studentId, cancellationToken) => await studentRepository.ExistsAsync(studentId, cancellationToken))
                .WithMessage((x) => $"StudentId = {x.StudentId} не существует");

            RuleFor(x => x.WorkId)
                .GreaterThan(0)
                .MustAsync(async (workId, cancellationToken) => await workRepository.ExistsAsync(workId, cancellationToken))
                .WithMessage((x) => $"WorkId = {x.WorkId} не существует");

            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("Оценка не может быть пустой")
                .MaximumLength(1).WithMessage("Оценка не может быть длиннее 1 символа");
        }
    }

    public class GetMarksByDisciplineAndGroupRequestValidator : AbstractValidator<GetMarksByDisciplineAndGroupRequest>
    {
        public GetMarksByDisciplineAndGroupRequestValidator(IDisciplineRepository disciplineRepository, IGroupRepository groupRepository)
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

    public class DeleteMarkByStudentAndWorkRequestValidator : AbstractValidator<DeleteMarkByStudentAndWorkRequest>
    {
        public DeleteMarkByStudentAndWorkRequestValidator(IStudentRepository studentRepository, IWorkRepository workRepository)
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0)
                .MustAsync(async (studentId, cancellationToken) => await studentRepository.ExistsAsync(studentId, cancellationToken))
                .WithMessage((x) => $"StudentId = {x.StudentId} не существует");

            RuleFor(x => x.WorkId)
                .GreaterThan(0)
                .MustAsync(async (workId, cancellationToken) => await workRepository.ExistsAsync(workId, cancellationToken))
                .WithMessage((x) => $"WorkId = {x.WorkId} не существует");
        }
    }
}
