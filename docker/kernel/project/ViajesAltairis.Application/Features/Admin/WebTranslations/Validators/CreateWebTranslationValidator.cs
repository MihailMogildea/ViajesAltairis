using FluentValidation;
using ViajesAltairis.Application.Features.Admin.WebTranslations.Commands;

namespace ViajesAltairis.Application.Features.Admin.WebTranslations.Validators;

public class CreateWebTranslationValidator : AbstractValidator<CreateWebTranslationCommand>
{
    public CreateWebTranslationValidator()
    {
        RuleFor(x => x.TranslationKey).NotEmpty().MaximumLength(150);
        RuleFor(x => x.LanguageId).GreaterThan(0);
        RuleFor(x => x.Value).NotEmpty();
    }
}
