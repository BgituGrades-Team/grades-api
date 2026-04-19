using BgituGrades.Application.Models.Student;
using BgituGrades.Domain.Interfaces;
using FluentValidation;

namespace BgituGrades.Application.Validators
{
    public class CreateStudentRequestValidator : AbstractValidator<CreateStudentRequest>
    {
        public CreateStudentRequestValidator(IGroupRepository groupRepository)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                    .WithMessage("Имя студента не может быть пустым")
                .MaximumLength(255)
                    .WithMessage("Имя студента не может быть длиннее 255 символов");

            RuleFor(x => x.GroupId)
                .MustAsync(async (groupId, cancellationToken) => await groupRepository.ExistsAsync(groupId, cancellationToken))
                    .WithMessage((x) => $"GroupId = {x.GroupId} не существует");

            RuleFor(x => x.OfficialId)
                .GreaterThan(0)
                    .WithMessage("OfficialId должен быть больше 0");
        }
    }

    public class UpdateStudentRequestValidator : AbstractValidator<UpdateStudentRequest>
    {
        public UpdateStudentRequestValidator(IGroupRepository groupRepository, IStudentRepository studentRepository)
        {
            RuleFor(x => x.Id)
                .MustAsync(async (id, cancellationToken) => await studentRepository.ExistsAsync(id, cancellationToken))
                    .WithMessage((x) => $"Id = {x.Id} не существует");

            RuleFor(x => x.Name)
                .NotEmpty()
                    .WithMessage("Имя студента не может быть пустым")
                .MaximumLength(255)
                    .WithMessage("Имя студента не может быть длиннее 255 символов");

            RuleFor(x => x.GroupId)
                .MustAsync(async (groupId, cancellationToken) => await groupRepository.ExistsAsync(groupId, cancellationToken))
                    .WithMessage((x) => $"GroupId = {x.GroupId} не существует");
        }
    }
    public class GetStudentsByGroupRequestValidator : AbstractValidator<GetStudentsByGroupRequest>
    {
        public GetStudentsByGroupRequestValidator()
        {
            RuleFor(x => x.GroupIds)
                .NotEmpty()
                    .WithMessage("GroupIds не может быть пустым");
        }
    }
}

