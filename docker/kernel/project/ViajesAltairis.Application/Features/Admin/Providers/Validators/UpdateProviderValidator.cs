using FluentValidation;
using ViajesAltairis.Application.Features.Admin.Providers.Commands;

namespace ViajesAltairis.Application.Features.Admin.Providers.Validators;

public class UpdateProviderValidator : AbstractValidator<UpdateProviderCommand>
{
    public UpdateProviderValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.TypeId).GreaterThan(0);
        RuleFor(x => x.CurrencyId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.ApiUrl).MaximumLength(500);
        RuleFor(x => x.ApiUsername).MaximumLength(150);
        RuleFor(x => x.Margin).GreaterThanOrEqualTo(0);
    }
}
