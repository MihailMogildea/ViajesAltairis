using FluentValidation;
using ViajesAltairis.Application.Features.Admin.CancellationPolicies.Commands;

namespace ViajesAltairis.Application.Features.Admin.CancellationPolicies.Validators;

public class UpdateCancellationPolicyValidator : AbstractValidator<UpdateCancellationPolicyCommand>
{
    public UpdateCancellationPolicyValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.HotelId).GreaterThan(0);
        RuleFor(x => x.FreeCancellationHours).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PenaltyPercentage).InclusiveBetween(0, 100);
    }
}
