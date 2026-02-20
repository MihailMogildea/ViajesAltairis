namespace ViajesAltairis.Domain.Entities;

public class RoomType : BaseEntity
{
    public string Name { get; set; } = null!;

    public ICollection<HotelProviderRoomType> HotelProviderRoomTypes { get; set; } = [];
}
