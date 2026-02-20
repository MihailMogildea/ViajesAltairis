namespace ViajesAltairis.Domain.Entities;

public class UserHotel : BaseEntity
{
    public long UserId { get; set; }
    public long HotelId { get; set; }

    public User User { get; set; } = null!;
    public Hotel Hotel { get; set; } = null!;
}
