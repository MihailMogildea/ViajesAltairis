using FluentValidation;
using ViajesAltairis.Application.Features.ExternalClient.Reservations.Commands;

namespace ViajesAltairis.Application.Features.ExternalClient.Reservations.Validators;

public class CancelPartnerReservationValidator : AbstractValidator<CancelPartnerReservationCommand>
{
    public CancelPartnerReservationValidator()
    {
        RuleFor(x => x.ReservationId).GreaterThan(0);
    }
}
