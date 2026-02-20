namespace ViajesAltairis.Application.Features.Admin.Statistics.Dtos;

public class RevenueByProviderDto
{
    public long ProviderId { get; init; }
    public string ProviderName { get; init; } = null!;
    public string CurrencyCode { get; init; } = null!;
    public decimal TotalRevenue { get; init; }
    public int ReservationCount { get; init; }
}
