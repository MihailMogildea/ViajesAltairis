using FluentValidation;
using ViajesAltairis.Application.Features.Admin.Taxes.Commands;

namespace ViajesAltairis.Application.Features.Admin.Taxes.Validators;

public class CreateTaxValidator : AbstractValidator<CreateTaxCommand>
{
    public CreateTaxValidator()
    {
        RuleFor(x => x.TaxTypeId).GreaterThan(0);
        RuleFor(x => x.Rate).GreaterThanOrEqualTo(0);
    }
}
