using FluentValidation;
using ViajesAltairis.Application.Features.Admin.Translations.Commands;

namespace ViajesAltairis.Application.Features.Admin.Translations.Validators;

public class UpdateTranslationValidator : AbstractValidator<UpdateTranslationCommand>
{
    public UpdateTranslationValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.EntityType).NotEmpty().MaximumLength(50);
        RuleFor(x => x.EntityId).GreaterThan(0);
        RuleFor(x => x.Field).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LanguageId).GreaterThan(0);
        RuleFor(x => x.Value).NotEmpty();
    }
}
