namespace ViajesAltairis.Application.Features.Admin.Statistics.Dtos;

public class ActiveSubscriptionsDto
{
    public int ActiveCount { get; init; }
    public int TotalUsers { get; init; }
    public decimal SubscriptionRate { get; init; }
}
