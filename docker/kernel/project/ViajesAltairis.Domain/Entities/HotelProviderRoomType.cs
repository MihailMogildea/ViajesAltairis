namespace ViajesAltairis.Domain.Entities;

public class HotelProviderRoomType : BaseEntity
{
    public long HotelProviderId { get; set; }
    public long RoomTypeId { get; set; }
    public byte Capacity { get; set; }
    public int Quantity { get; set; }
    public decimal PricePerNight { get; set; }
    public long CurrencyId { get; set; }
    public long ExchangeRateId { get; set; }
    public bool Enabled { get; set; }

    public HotelProvider HotelProvider { get; set; } = null!;
    public RoomType RoomType { get; set; } = null!;
    public Currency Currency { get; set; } = null!;
    public ExchangeRate ExchangeRate { get; set; } = null!;
    public ICollection<HotelProviderRoomTypeAmenity> HotelProviderRoomTypeAmenities { get; set; } = [];
    public ICollection<HotelProviderRoomTypeBoard> HotelProviderRoomTypeBoards { get; set; } = [];
    public ICollection<RoomImage> RoomImages { get; set; } = [];
    public ICollection<ReservationLine> ReservationLines { get; set; } = [];
}
