using FluentValidation;
using ViajesAltairis.Application.Features.Admin.Languages.Commands;

namespace ViajesAltairis.Application.Features.Admin.Languages.Validators;

public class UpdateLanguageValidator : AbstractValidator<UpdateLanguageCommand>
{
    public UpdateLanguageValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.IsoCode).NotEmpty().Length(2);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
    }
}
