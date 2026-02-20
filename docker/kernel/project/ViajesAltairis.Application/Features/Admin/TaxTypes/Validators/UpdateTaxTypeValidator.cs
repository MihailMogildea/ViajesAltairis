using FluentValidation;
using ViajesAltairis.Application.Features.Admin.TaxTypes.Commands;

namespace ViajesAltairis.Application.Features.Admin.TaxTypes.Validators;

public class UpdateTaxTypeValidator : AbstractValidator<UpdateTaxTypeCommand>
{
    public UpdateTaxTypeValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
    }
}
