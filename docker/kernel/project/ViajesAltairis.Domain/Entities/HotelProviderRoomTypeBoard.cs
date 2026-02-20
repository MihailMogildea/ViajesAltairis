namespace ViajesAltairis.Domain.Entities;

public class HotelProviderRoomTypeBoard
{
    public long Id { get; set; }
    public long HotelProviderRoomTypeId { get; set; }
    public long BoardTypeId { get; set; }
    public decimal PricePerNight { get; set; }
    public bool Enabled { get; set; }

    public HotelProviderRoomType HotelProviderRoomType { get; set; } = null!;
    public BoardType BoardType { get; set; } = null!;
}
