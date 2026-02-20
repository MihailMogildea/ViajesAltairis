namespace ViajesAltairis.Application.Features.Admin.Statistics.Dtos;

public class OccupancyDto
{
    public long HotelId { get; init; }
    public string HotelName { get; init; } = null!;
    public int BookedRoomNights { get; init; }
    public int TotalRoomNights { get; init; }
    public decimal OccupancyRate { get; init; }
}
