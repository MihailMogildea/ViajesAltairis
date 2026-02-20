using FluentValidation;
using ViajesAltairis.Application.Features.Admin.Languages.Commands;

namespace ViajesAltairis.Application.Features.Admin.Languages.Validators;

public class CreateLanguageValidator : AbstractValidator<CreateLanguageCommand>
{
    public CreateLanguageValidator()
    {
        RuleFor(x => x.IsoCode).NotEmpty().Length(2);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
    }
}
