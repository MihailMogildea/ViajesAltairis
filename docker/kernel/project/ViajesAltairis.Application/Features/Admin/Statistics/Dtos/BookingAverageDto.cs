namespace ViajesAltairis.Application.Features.Admin.Statistics.Dtos;

public class BookingAverageDto
{
    public decimal AverageValue { get; init; }
    public decimal AverageNights { get; init; }
    public int TotalBookings { get; init; }
}
