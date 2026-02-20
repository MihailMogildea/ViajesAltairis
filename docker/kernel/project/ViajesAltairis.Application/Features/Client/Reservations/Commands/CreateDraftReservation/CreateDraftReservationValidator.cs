using FluentValidation;

namespace ViajesAltairis.Application.Features.Client.Reservations.Commands.CreateDraftReservation;

public class CreateDraftReservationValidator : AbstractValidator<CreateDraftReservationCommand>
{
    public CreateDraftReservationValidator()
    {
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3);
    }
}
