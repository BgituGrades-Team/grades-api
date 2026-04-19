using BgituGrades.Application.Models.Setting;
using FluentValidation;

namespace BgituGrades.Application.Validators
{
    public class SettingRequestValidator : AbstractValidator<UpdateSettingRequest>
    {
        public SettingRequestValidator()
        {
            RuleFor(x => x.CalendarUrl)
                .NotEmpty().WithMessage("Ссылка на календарный учебный график не должна быть пустая.");
                //.Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).WithMessage("Ссылка должна быть валидной.");
        }
    }
}
