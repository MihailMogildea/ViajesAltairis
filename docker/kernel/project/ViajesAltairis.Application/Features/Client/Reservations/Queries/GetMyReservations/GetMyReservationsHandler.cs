using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Reservations.Queries.GetMyReservations;

public class GetMyReservationsHandler : IRequestHandler<GetMyReservationsQuery, GetMyReservationsResponse>
{
    private readonly IReservationApiClient _reservationApi;
    private readonly ICurrentUserService _currentUser;

    public GetMyReservationsHandler(IReservationApiClient reservationApi, ICurrentUserService currentUser)
    {
        _reservationApi = reservationApi;
        _currentUser = currentUser;
    }

    public async Task<GetMyReservationsResponse> Handle(GetMyReservationsQuery request, CancellationToken cancellationToken)
    {
        var result = await _reservationApi.GetByUserAsync(
            _currentUser.UserId!.Value,
            request.Page,
            request.PageSize,
            request.Status,
            cancellationToken);

        return new GetMyReservationsResponse
        {
            TotalCount = result.TotalCount,
            Reservations = result.Reservations.Select(r => new ReservationSummaryDto
            {
                Id = r.Id,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                TotalAmount = r.TotalAmount,
                Currency = r.CurrencyCode,
                LineCount = r.LineCount
            }).ToList()
        };
    }
}
