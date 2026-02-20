using MediatR;

namespace ViajesAltairis.Application.Features.Client.Subscriptions.Queries.GetMySubscription;

public class GetMySubscriptionQuery : IRequest<GetMySubscriptionResponse>
{
}

public class GetMySubscriptionResponse
{
    public long? SubscriptionId { get; set; }
    public long? SubscriptionTypeId { get; set; }
    public string? PlanName { get; set; }
    public decimal? Discount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
}
