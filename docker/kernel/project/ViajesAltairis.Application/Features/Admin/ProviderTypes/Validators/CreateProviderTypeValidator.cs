using FluentValidation;
using ViajesAltairis.Application.Features.Admin.ProviderTypes.Commands;

namespace ViajesAltairis.Application.Features.Admin.ProviderTypes.Validators;

public class CreateProviderTypeValidator : AbstractValidator<CreateProviderTypeCommand>
{
    public CreateProviderTypeValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
    }
}
