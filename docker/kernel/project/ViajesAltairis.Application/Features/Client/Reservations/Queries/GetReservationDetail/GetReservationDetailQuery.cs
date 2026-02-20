using MediatR;

namespace ViajesAltairis.Application.Features.Client.Reservations.Queries.GetReservationDetail;

public class GetReservationDetailQuery : IRequest<GetReservationDetailResponse>
{
    public long ReservationId { get; set; }
}

public class GetReservationDetailResponse
{
    public long Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalDiscount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal ExchangeRate { get; set; }
    public string? PromoCode { get; set; }
    public List<ReservationLineDto> Lines { get; set; } = new();
}

public class ReservationLineDto
{
    public long Id { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public string BoardType { get; set; } = string.Empty;
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int GuestCount { get; set; }
    public decimal LineTotal { get; set; }
}
