using FluentValidation;
using ViajesAltairis.Application.Features.Admin.Currencies.Commands;

namespace ViajesAltairis.Application.Features.Admin.Currencies.Validators;

public class CreateCurrencyValidator : AbstractValidator<CreateCurrencyCommand>
{
    public CreateCurrencyValidator()
    {
        RuleFor(x => x.IsoCode).NotEmpty().Length(3);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Symbol).NotEmpty().MaximumLength(5);
    }
}
