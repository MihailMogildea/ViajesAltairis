using FluentValidation;
using ViajesAltairis.Application.Features.Admin.Countries.Commands;

namespace ViajesAltairis.Application.Features.Admin.Countries.Validators;

public class CreateCountryValidator : AbstractValidator<CreateCountryCommand>
{
    public CreateCountryValidator()
    {
        RuleFor(x => x.IsoCode).NotEmpty().Length(2);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CurrencyId).GreaterThan(0);
    }
}
