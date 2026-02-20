using MediatR;

namespace ViajesAltairis.Application.Features.Client.Reservations.Commands.CreateDraftReservation;

public class CreateDraftReservationCommand : IRequest<long>
{
    public string CurrencyCode { get; set; } = string.Empty;
    public string? PromoCode { get; set; }
}
