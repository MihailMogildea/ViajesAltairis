using FluentValidation;

namespace ViajesAltairis.Application.Features.Client.Subscriptions.Commands.Subscribe;

public class SubscribeValidator : AbstractValidator<SubscribeCommand>
{
    public SubscribeValidator()
    {
        RuleFor(x => x.SubscriptionTypeId).GreaterThan(0);
    }
}
