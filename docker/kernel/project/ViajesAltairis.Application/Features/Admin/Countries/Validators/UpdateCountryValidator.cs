using FluentValidation;
using ViajesAltairis.Application.Features.Admin.Countries.Commands;

namespace ViajesAltairis.Application.Features.Admin.Countries.Validators;

public class UpdateCountryValidator : AbstractValidator<UpdateCountryCommand>
{
    public UpdateCountryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.IsoCode).NotEmpty().Length(2);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CurrencyId).GreaterThan(0);
    }
}
