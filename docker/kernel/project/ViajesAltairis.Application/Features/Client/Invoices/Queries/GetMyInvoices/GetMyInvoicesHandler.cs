using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Invoices.Queries.GetMyInvoices;

public class GetMyInvoicesHandler : IRequestHandler<GetMyInvoicesQuery, GetMyInvoicesResponse>
{
    private readonly IReservationApiClient _reservationApi;
    private readonly ICurrentUserService _currentUser;

    public GetMyInvoicesHandler(IReservationApiClient reservationApi, ICurrentUserService currentUser)
    {
        _reservationApi = reservationApi;
        _currentUser = currentUser;
    }

    public async Task<GetMyInvoicesResponse> Handle(GetMyInvoicesQuery request, CancellationToken cancellationToken)
    {
        var result = await _reservationApi.GetInvoicesByUserAsync(
            _currentUser.UserId!.Value, request.Page, request.PageSize, cancellationToken);

        var invoices = result.Invoices.Select(i => new InvoiceSummaryDto
        {
            Id = i.Id,
            InvoiceNumber = i.InvoiceNumber,
            Status = i.Status,
            TotalAmount = i.TotalAmount,
            Currency = i.Currency,
            IssuedAt = i.IssuedAt
        }).ToList();

        return new GetMyInvoicesResponse
        {
            Invoices = invoices,
            TotalCount = result.TotalCount
        };
    }
}
