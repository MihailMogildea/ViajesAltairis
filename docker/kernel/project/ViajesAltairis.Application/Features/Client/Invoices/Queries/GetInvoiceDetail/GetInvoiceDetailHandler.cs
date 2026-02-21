using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Invoices.Queries.GetInvoiceDetail;

public class GetInvoiceDetailHandler : IRequestHandler<GetInvoiceDetailQuery, GetInvoiceDetailResponse>
{
    private readonly IReservationApiClient _reservationApi;
    private readonly ICurrentUserService _currentUser;
    private readonly ITranslationService _translationService;

    public GetInvoiceDetailHandler(
        IReservationApiClient reservationApi,
        ICurrentUserService currentUser,
        ITranslationService translationService)
    {
        _reservationApi = reservationApi;
        _currentUser = currentUser;
        _translationService = translationService;
    }

    public async Task<GetInvoiceDetailResponse> Handle(GetInvoiceDetailQuery request, CancellationToken cancellationToken)
    {
        var result = await _reservationApi.GetInvoiceByIdAsync(
            request.InvoiceId, _currentUser.UserId!.Value, cancellationToken);

        if (result == null)
            throw new KeyNotFoundException($"Invoice {request.InvoiceId} not found.");

        var status = result.Status;
        var langId = _currentUser.LanguageId;
        var statusNames = await _translationService.ResolveAsync(
            "invoice_status", [result.StatusId], langId, "name", cancellationToken);
        if (statusNames.TryGetValue(result.StatusId, out var translatedStatus))
            status = translatedStatus;

        return new GetInvoiceDetailResponse
        {
            Id = result.Id,
            InvoiceNumber = result.InvoiceNumber,
            Status = status,
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
