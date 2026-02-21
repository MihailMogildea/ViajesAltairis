namespace ViajesAltairis.Application.Features.Admin.Reviews.Dtos;

public class ReviewDto
{
    public long Id { get; init; }
    public long ReservationId { get; init; }
    public long UserId { get; init; }
    public string UserEmail { get; init; } = null!;
    public long HotelId { get; init; }
    public int Rating { get; init; }
    public string? Title { get; init; }
    public string? Comment { get; init; }
    public bool Visible { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
