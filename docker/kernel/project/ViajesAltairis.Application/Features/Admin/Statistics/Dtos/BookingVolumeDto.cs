namespace ViajesAltairis.Application.Features.Admin.Statistics.Dtos;

public class BookingVolumeDto
{
    public string Period { get; init; } = null!;
    public int BookingCount { get; init; }
}
