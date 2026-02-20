using FluentValidation;
using ViajesAltairis.Application.Features.ExternalClient.Reservations.Commands;

namespace ViajesAltairis.Application.Features.ExternalClient.Reservations.Validators;

public class SubmitPartnerReservationValidator : AbstractValidator<SubmitPartnerReservationCommand>
{
    public SubmitPartnerReservationValidator()
    {
        RuleFor(x => x.ReservationId).GreaterThan(0);
        RuleFor(x => x.PaymentMethodId).GreaterThan(0);
    }
}
