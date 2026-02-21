using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Reservations.Queries.GetReservationDetail;

public class GetReservationDetailHandler : IRequestHandler<GetReservationDetailQuery, GetReservationDetailResponse>
{
    private readonly IReservationApiClient _reservationApi;
    private readonly ICurrentUserService _currentUser;

    public GetReservationDetailHandler(IReservationApiClient reservationApi, ICurrentUserService currentUser)
    {
        _reservationApi = reservationApi;
        _currentUser = currentUser;
    }

    public async Task<GetReservationDetailResponse> Handle(GetReservationDetailQuery request, CancellationToken cancellationToken)
    {
        var result = await _reservationApi.GetByIdAsync(request.ReservationId, cancellationToken);
        if (result == null)
            throw new KeyNotFoundException($"Reservation {request.ReservationId} not found.");

        var userId = _currentUser.UserId!.Value;
        if (result.BookedByUserId != userId && result.OwnerUserId != userId)
            throw new UnauthorizedAccessException("This reservation does not belong to you.");

        return new GetReservationDetailResponse
        {
            Id = result.Id,
            Status = result.Status,
            CreatedAt = result.CreatedAt,
            TotalAmount = result.TotalAmount,
            TotalDiscount = result.TotalDiscount,
            Currency = result.CurrencyCode,
            ExchangeRate = result.ExchangeRate,
            PromoCode = result.PromoCode,
            Lines = result.Lines.Select(l => new ReservationLineDto
            {
                Id = l.Id,
                HotelName = l.HotelName,
                RoomType = l.RoomType,
                BoardType = l.BoardType,
                CheckIn = l.CheckIn,
                CheckOut = l.CheckOut,
                GuestCount = l.GuestCount,
                LineTotal = l.LineTotal
            }).ToList()
        };
    }
}
