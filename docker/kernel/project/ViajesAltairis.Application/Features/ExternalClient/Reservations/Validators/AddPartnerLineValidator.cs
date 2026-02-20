using FluentValidation;
using ViajesAltairis.Application.Features.ExternalClient.Reservations.Commands;

namespace ViajesAltairis.Application.Features.ExternalClient.Reservations.Validators;

public class AddPartnerLineValidator : AbstractValidator<AddPartnerLineCommand>
{
    public AddPartnerLineValidator()
    {
        RuleFor(x => x.ReservationId).GreaterThan(0);
        RuleFor(x => x.RoomConfigurationId).GreaterThan(0);
        RuleFor(x => x.BoardTypeId).GreaterThan(0);
        RuleFor(x => x.CheckIn).NotEmpty();
        RuleFor(x => x.CheckOut).NotEmpty()
            .GreaterThan(x => x.CheckIn).WithMessage("Check-out must be after check-in.");
        RuleFor(x => x.GuestCount).GreaterThan(0);
    }
}
