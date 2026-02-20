using FluentValidation;
using ViajesAltairis.Application.Features.Admin.UserSubscriptions.Commands;

namespace ViajesAltairis.Application.Features.Admin.UserSubscriptions.Validators;

public class AssignUserSubscriptionValidator : AbstractValidator<AssignUserSubscriptionCommand>
{
    public AssignUserSubscriptionValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.SubscriptionTypeId).GreaterThan(0);
        RuleFor(x => x.StartDate).NotEmpty();
    }
}
