namespace ViajesAltairis.Application.Features.Admin.Statistics.Dtos;

public class SubscriptionMrrDto
{
    public string SubscriptionName { get; init; } = null!;
    public string CurrencyCode { get; init; } = null!;
    public int ActiveCount { get; init; }
    public decimal MonthlyRevenue { get; init; }
}
