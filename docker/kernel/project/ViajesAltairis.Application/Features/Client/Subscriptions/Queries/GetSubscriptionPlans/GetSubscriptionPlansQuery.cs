using MediatR;

namespace ViajesAltairis.Application.Features.Client.Subscriptions.Queries.GetSubscriptionPlans;

public class GetSubscriptionPlansQuery : IRequest<GetSubscriptionPlansResponse>
{
}

public class GetSubscriptionPlansResponse
{
    public List<SubscriptionPlanDto> Plans { get; set; } = new();
}

public class SubscriptionPlanDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public long CurrencyId { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
}
