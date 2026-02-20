namespace ViajesAltairis.Domain.Entities;

public class RoomImage : BaseEntity
{
    public long HotelProviderRoomTypeId { get; set; }
    public string Url { get; set; } = null!;
    public string? AltText { get; set; }
    public int SortOrder { get; set; }

    public HotelProviderRoomType HotelProviderRoomType { get; set; } = null!;
}
