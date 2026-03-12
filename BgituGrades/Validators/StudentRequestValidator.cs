using FluentValidation;
using BgituGrades.Models.Student;

namespace BgituGrades.Validators
{
    public class CreateStudentRequestValidator : AbstractValidator<CreateStudentRequest>
    {
        public CreateStudentRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                    .WithMessage("Имя студента не может быть пустым")
                    .WithErrorCode("STUDENT_NAME_EMPTY")
                .MaximumLength(255)
                    .WithMessage("Имя студента не может быть длиннее 255 символов")
                    .WithErrorCode("STUDENT_NAME_TOO_LONG");

            RuleFor(x => x.GroupId)
                .GreaterThan(0)
                    .WithMessage("GroupId должен быть больше 0")
                    .WithErrorCode("INVALID_GROUP_ID");
            RuleFor(x => x.OfficialId)
                .GreaterThan(0)
                    .WithMessage("OfficialId должен быть больше 0")
                    .WithErrorCode("INVALID_OFFICIAL_ID");
        }
    }

    public class UpdateStudentRequestValidator : AbstractValidator<UpdateStudentRequest>
    {
        public UpdateStudentRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                    .WithMessage("ID студента должен быть больше 0")
                    .WithErrorCode("INVALID_STUDENT_ID");

            RuleFor(x => x.Name)
                .NotEmpty()
                    .WithMessage("Имя студента не может быть пустым")
                    .WithErrorCode("STUDENT_NAME_EMPTY")
                .MaximumLength(255)
                    .WithMessage("Имя студента не может быть длиннее 255 символов")
                    .WithErrorCode("STUDENT_NAME_TOO_LONG");

            RuleFor(x => x.GroupId)
                .GreaterThan(0)
                    .WithMessage("GroupId должен быть больше 0")
                    .WithErrorCode("INVALID_GROUP_ID");
        }
    }
    public class GetStudentsByGroupRequestValidator : AbstractValidator<GetStudentsByGroupRequest>
    {
        public GetStudentsByGroupRequestValidator()
        {
            RuleFor(x => x.GroupId)
                .GreaterThan(0)
                    .WithMessage("GroupId должен быть больше 0")
                    .WithErrorCode("INVALID_GROUP_ID");
        }
    }
}

