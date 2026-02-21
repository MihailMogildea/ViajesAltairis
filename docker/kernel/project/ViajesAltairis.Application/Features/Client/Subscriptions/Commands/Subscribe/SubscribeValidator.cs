using FluentValidation;

namespace ViajesAltairis.Application.Features.Client.Subscriptions.Commands.Subscribe;

public class SubscribeValidator : AbstractValidator<SubscribeCommand>
{
    public SubscribeValidator()
    {
        RuleFor(x => x.SubscriptionTypeId).GreaterThan(0);
        RuleFor(x => x.PaymentMethodId).GreaterThan(0);
        RuleFor(x => x.CardNumber).NotEmpty();
        RuleFor(x => x.CardExpiry).NotEmpty();
        RuleFor(x => x.CardCvv).NotEmpty();
    }
}
