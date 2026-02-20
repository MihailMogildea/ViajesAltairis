namespace ViajesAltairis.Application.Features.Admin.Statistics.Dtos;

public class BookingsByStatusDto
{
    public string StatusName { get; init; } = null!;
    public int BookingCount { get; init; }
}
