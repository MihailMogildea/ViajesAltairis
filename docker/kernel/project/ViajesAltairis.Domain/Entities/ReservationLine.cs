namespace ViajesAltairis.Domain.Entities;

public class ReservationLine : AuditableEntity
{
    public long ReservationId { get; set; }
    public long HotelProviderRoomTypeId { get; set; }
    public long BoardTypeId { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int NumRooms { get; set; }
    public byte NumGuests { get; set; }
    public decimal PricePerNight { get; set; }
    public decimal BoardPricePerNight { get; set; }
    public int NumNights { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal MarginAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalPrice { get; set; }
    public long CurrencyId { get; set; }
    public long ExchangeRateId { get; set; }

    public Reservation Reservation { get; set; } = null!;
    public HotelProviderRoomType HotelProviderRoomType { get; set; } = null!;
    public BoardType BoardType { get; set; } = null!;
    public Currency Currency { get; set; } = null!;
    public ExchangeRate ExchangeRate { get; set; } = null!;
    public ICollection<ReservationGuest> ReservationGuests { get; set; } = [];
}
