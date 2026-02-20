using FluentValidation;
using ViajesAltairis.Application.Features.Admin.Taxes.Commands;

namespace ViajesAltairis.Application.Features.Admin.Taxes.Validators;

public class UpdateTaxValidator : AbstractValidator<UpdateTaxCommand>
{
    public UpdateTaxValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.TaxTypeId).GreaterThan(0);
        RuleFor(x => x.Rate).GreaterThanOrEqualTo(0);
    }
}
