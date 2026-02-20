namespace ViajesAltairis.Domain.Entities;

public class ReviewResponse : BaseEntity
{
    public long ReviewId { get; set; }
    public long UserId { get; set; }
    public string Comment { get; set; } = null!;

    public Review Review { get; set; } = null!;
    public User User { get; set; } = null!;
}
