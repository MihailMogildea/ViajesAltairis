namespace ViajesAltairis.Application.Features.Admin.Statistics.Dtos;

public class RevenuePeriodDto
{
    public string Period { get; init; } = null!;
    public string CurrencyCode { get; init; } = null!;
    public decimal TotalRevenue { get; init; }
    public int ReservationCount { get; init; }
}
