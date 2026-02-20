using FluentValidation;
using ViajesAltairis.Application.Features.Admin.ProviderTypes.Commands;

namespace ViajesAltairis.Application.Features.Admin.ProviderTypes.Validators;

public class UpdateProviderTypeValidator : AbstractValidator<UpdateProviderTypeCommand>
{
    public UpdateProviderTypeValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
    }
}
