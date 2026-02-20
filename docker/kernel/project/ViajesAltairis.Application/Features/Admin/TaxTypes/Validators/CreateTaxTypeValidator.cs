using FluentValidation;
using ViajesAltairis.Application.Features.Admin.TaxTypes.Commands;

namespace ViajesAltairis.Application.Features.Admin.TaxTypes.Validators;

public class CreateTaxTypeValidator : AbstractValidator<CreateTaxTypeCommand>
{
    public CreateTaxTypeValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
    }
}
