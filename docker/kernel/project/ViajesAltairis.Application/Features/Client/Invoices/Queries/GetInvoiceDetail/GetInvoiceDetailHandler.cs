using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Invoices.Queries.GetInvoiceDetail;

public class GetInvoiceDetailHandler : IRequestHandler<GetInvoiceDetailQuery, GetInvoiceDetailResponse>
{
    private readonly IReservationApiClient _reservationApi;
    private readonly ICurrentUserService _currentUser;

    public GetInvoiceDetailHandler(IReservationApiClient reservationApi, ICurrentUserService currentUser)
    {
        _reservationApi = reservationApi;
        _currentUser = currentUser;
    }

    public async Task<GetInvoiceDetailResponse> Handle(GetInvoiceDetailQuery request, CancellationToken cancellationToken)
    {
        var result = await _reservationApi.GetInvoiceByIdAsync(
            request.InvoiceId, _currentUser.UserId!.Value, cancellationToken);

        if (result == null)
            throw new KeyNotFoundException($"Invoice {request.InvoiceId} not found.");

        return new GetInvoiceDetailResponse
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
            ReservationId = result.ReservationId
        };
    }
}
