using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Reservations.Commands.SubmitReservation;

public class SubmitReservationHandler : IRequestHandler<SubmitReservationCommand, SubmitReservationResponse>
{
    private readonly IReservationApiClient _reservationApi;

    public SubmitReservationHandler(IReservationApiClient reservationApi)
    {
        _reservationApi = reservationApi;
    }

    public async Task<SubmitReservationResponse> Handle(SubmitReservationCommand request, CancellationToken cancellationToken)
    {
        var result = await _reservationApi.SubmitAsync(
            request.ReservationId,
            request.PaymentMethodId,
            request.CardNumber,
            request.CardExpiry,
            request.CardCvv,
            request.CardHolderName,
            cancellationToken);

        return new SubmitReservationResponse
        {
            ReservationId = result.ReservationId,
            Status = result.Status,
            TotalAmount = result.TotalAmount,
            Currency = result.CurrencyCode
        };
    }
}
