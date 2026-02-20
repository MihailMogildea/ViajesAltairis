using FluentValidation;
using ViajesAltairis.Application.Features.Admin.WebTranslations.Commands;

namespace ViajesAltairis.Application.Features.Admin.WebTranslations.Validators;

public class UpdateWebTranslationValidator : AbstractValidator<UpdateWebTranslationCommand>
{
    public UpdateWebTranslationValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.TranslationKey).NotEmpty().MaximumLength(150);
        RuleFor(x => x.LanguageId).GreaterThan(0);
        RuleFor(x => x.Value).NotEmpty();
    }
}
