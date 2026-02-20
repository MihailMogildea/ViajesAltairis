namespace ViajesAltairis.Application.Features.Admin.Statistics.Dtos;

public class RevenueByHotelDto
{
    public long HotelId { get; init; }
    public string HotelName { get; init; } = null!;
    public string CurrencyCode { get; init; } = null!;
    public decimal TotalRevenue { get; init; }
    public int ReservationCount { get; init; }
}
