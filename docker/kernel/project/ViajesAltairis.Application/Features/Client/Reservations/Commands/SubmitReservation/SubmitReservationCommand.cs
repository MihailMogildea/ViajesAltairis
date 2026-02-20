using MediatR;

namespace ViajesAltairis.Application.Features.Client.Reservations.Commands.SubmitReservation;

public class SubmitReservationCommand : IRequest<SubmitReservationResponse>
{
    public long ReservationId { get; set; }
    public long PaymentMethodId { get; set; }
    public string? CardNumber { get; set; }
    public string? CardExpiry { get; set; }
    public string? CardCvv { get; set; }
    public string? CardHolderName { get; set; }
}

public class SubmitReservationResponse
{
    public long ReservationId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
}
