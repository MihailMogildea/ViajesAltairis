namespace ViajesAltairis.Application.Features.Admin.Statistics.Dtos;

public class ReviewStatsDto
{
    public decimal AverageRating { get; init; }
    public int TotalReviews { get; init; }
    public int Rating1 { get; init; }
    public int Rating2 { get; init; }
    public int Rating3 { get; init; }
    public int Rating4 { get; init; }
    public int Rating5 { get; init; }
}
