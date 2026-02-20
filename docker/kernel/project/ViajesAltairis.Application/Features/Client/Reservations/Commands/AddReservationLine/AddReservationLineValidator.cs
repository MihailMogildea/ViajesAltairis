using FluentValidation;

namespace ViajesAltairis.Application.Features.Client.Reservations.Commands.AddReservationLine;

public class AddReservationLineValidator : AbstractValidator<AddReservationLineCommand>
{
    public AddReservationLineValidator()
    {
        RuleFor(x => x.RoomConfigurationId).GreaterThan(0);
        RuleFor(x => x.BoardTypeId).GreaterThan(0);
        RuleFor(x => x.CheckIn).GreaterThan(DateTime.MinValue);
        RuleFor(x => x.CheckOut).GreaterThan(x => x.CheckIn);
        RuleFor(x => x.GuestCount).GreaterThan(0);
    }
}
