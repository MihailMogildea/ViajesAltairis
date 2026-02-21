namespace ViajesAltairis.Application.Features.Admin.Reservations.Dtos;

public class ReservationLineAdminDto
{
    public long ReservationLineId { get; init; }
    public long ReservationId { get; init; }
    public long HotelId { get; init; }
    public string HotelName { get; init; } = null!;
    public long RoomTypeId { get; init; }
    public string RoomTypeName { get; init; } = null!;
    public long BoardTypeId { get; init; }
    public string BoardTypeName { get; init; } = null!;
    public long ProviderId { get; init; }
    public string ProviderName { get; init; } = null!;
    public DateTime CheckInDate { get; init; }
    public DateTime CheckOutDate { get; init; }
    public int NumRooms { get; init; }
    public int NumGuests { get; init; }
    public decimal PricePerNight { get; init; }
    public decimal BoardPricePerNight { get; init; }
    public int NumNights { get; init; }
    public decimal Subtotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal MarginAmount { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TotalPrice { get; init; }
    public string CurrencyCode { get; init; } = null!;
}
