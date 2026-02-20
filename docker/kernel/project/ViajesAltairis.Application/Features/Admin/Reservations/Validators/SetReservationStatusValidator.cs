using FluentValidation;
using ViajesAltairis.Application.Features.Admin.Reservations.Commands;

namespace ViajesAltairis.Application.Features.Admin.Reservations.Validators;

public class SetReservationStatusValidator : AbstractValidator<SetReservationStatusCommand>
{
    public SetReservationStatusValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.StatusId).GreaterThan(0);
    }
}
