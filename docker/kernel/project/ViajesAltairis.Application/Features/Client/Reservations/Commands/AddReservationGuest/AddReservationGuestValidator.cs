using FluentValidation;

namespace ViajesAltairis.Application.Features.Client.Reservations.Commands.AddReservationGuest;

public class AddReservationGuestValidator : AbstractValidator<AddReservationGuestCommand>
{
    public AddReservationGuestValidator()
    {
        RuleFor(x => x.ReservationId).GreaterThan(0);
        RuleFor(x => x.LineId).GreaterThan(0);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
    }
}
