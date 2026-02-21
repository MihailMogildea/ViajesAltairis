using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Invoices.Commands.GenerateInvoice;

public class GenerateInvoiceHandler : IRequestHandler<GenerateInvoiceCommand, GenerateInvoiceResponse>
{
    private readonly IReservationApiClient _reservationApi;
    private readonly ICurrentUserService _currentUser;

    public GenerateInvoiceHandler(IReservationApiClient reservationApi, ICurrentUserService currentUser)
    {
        _reservationApi = reservationApi;
        _currentUser = currentUser;
    }

    public async Task<GenerateInvoiceResponse> Handle(GenerateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var result = await _reservationApi.CreateInvoiceAsync(
            request.ReservationId,
            _currentUser.UserId!.Value,
            cancellationToken);

        return new GenerateInvoiceResponse
        {
            Id = result.Id,
            InvoiceNumber = result.InvoiceNumber,
            Status = result.Status,
            SubTotal = result.SubTotal,
            TaxAmount = result.TaxAmount,
            TotalAmount = result.TotalAmount,
            Currency = result.Currency,
            ExchangeRateToEur = result.ExchangeRateToEur,
            IssuedAt = result.IssuedAt,
            PaidAt = result.PaidAt,
            InvoiceReservationId = result.ReservationId
        };
    }
}
