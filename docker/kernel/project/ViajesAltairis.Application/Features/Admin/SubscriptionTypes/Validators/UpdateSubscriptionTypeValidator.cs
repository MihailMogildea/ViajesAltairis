using FluentValidation;
using ViajesAltairis.Application.Features.Admin.SubscriptionTypes.Commands;

namespace ViajesAltairis.Application.Features.Admin.SubscriptionTypes.Validators;

public class UpdateSubscriptionTypeValidator : AbstractValidator<UpdateSubscriptionTypeCommand>
{
    public UpdateSubscriptionTypeValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PricePerMonth).GreaterThan(0);
        RuleFor(x => x.Discount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CurrencyId).GreaterThan(0);
    }
}
