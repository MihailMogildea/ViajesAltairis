namespace ViajesAltairis.Domain.Entities;

public class BoardType
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<HotelProviderRoomTypeBoard> HotelProviderRoomTypeBoards { get; set; } = [];
    public ICollection<ReservationLine> ReservationLines { get; set; } = [];
}
