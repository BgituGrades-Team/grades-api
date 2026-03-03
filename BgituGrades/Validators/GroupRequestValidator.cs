using FluentValidation;
using BgituGrades.Models.Group;

namespace BgituGrades.Validators
{
    /// <summary>
    /// Валидатор для создания группы.
    /// 
    /// Правила валидации:
    /// - Имя: обязательное поле, максимум 255 символов
    /// - StudyStartDate: должна быть раньше StudyEndDate
    /// - StartWeekNumber: должен быть от 1 до 52
    /// </summary>
    public class CreateGroupRequestValidator : AbstractValidator<CreateGroupRequest>
    {
        public CreateGroupRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                    .WithMessage("Имя группы не может быть пустым")
                    .WithErrorCode("GROUP_NAME_EMPTY")
                .MaximumLength(255)
                    .WithMessage("Имя группы не может быть длиннее 255 символов")
                    .WithErrorCode("GROUP_NAME_TOO_LONG");

            RuleFor(x => x.StudyStartDate)
                .LessThan(x => x.StudyEndDate)
                    .WithMessage("Дата начала должна быть раньше даты окончания")
                    .WithErrorCode("INVALID_DATE_RANGE");

            RuleFor(x => x.StartWeekNumber)
                .GreaterThan(0)
                    .WithMessage("Номер недели должен быть больше 0")
                    .WithErrorCode("INVALID_WEEK_NUMBER")
                .LessThanOrEqualTo(52)
                    .WithMessage("Номер недели не должен быть больше 52")
                    .WithErrorCode("WEEK_NUMBER_OUT_OF_RANGE");
        }
    }

    /// <summary>
    /// Валидатор для обновления группы.
    /// </summary>
    public class UpdateGroupRequestValidator : AbstractValidator<UpdateGroupRequest>
    {
        public UpdateGroupRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                    .WithMessage("ID группы должен быть больше 0")
                    .WithErrorCode("INVALID_GROUP_ID");

            RuleFor(x => x.Name)
                .NotEmpty()
                    .WithMessage("Имя группы не может быть пустым")
                    .WithErrorCode("GROUP_NAME_EMPTY")
                .MaximumLength(255)
                    .WithMessage("Имя группы не может быть длиннее 255 символов")
                    .WithErrorCode("GROUP_NAME_TOO_LONG");

            RuleFor(x => x.StudyStartDate)
                .LessThan(x => x.StudyEndDate)
                    .WithMessage("Дата начала должна быть раньше даты окончания")
                    .WithErrorCode("INVALID_DATE_RANGE");

            RuleFor(x => x.StartWeekNumber)
                .GreaterThan(0)
                    .WithMessage("Номер недели должен быть больше 0")
                    .WithErrorCode("INVALID_WEEK_NUMBER")
                .LessThanOrEqualTo(52)
                    .WithMessage("Номер недели не должен быть больше 52")
                    .WithErrorCode("WEEK_NUMBER_OUT_OF_RANGE");
        }
    }

    /// <summary>
    /// Валидатор для получения групп по дисциплине.
    /// </summary>
    public class GetGroupsByDisciplineRequestValidator : AbstractValidator<GetGroupsByDisciplineRequest>
    {
        public GetGroupsByDisciplineRequestValidator()
        {
            RuleFor(x => x.DisciplineId)
                .GreaterThan(0)
                    .WithMessage("DisciplineId должен быть больше 0")
                    .WithErrorCode("INVALID_DISCIPLINE_ID");
        }
    }
}
