using MediatR;

namespace ViajesAltairis.Application.Features.Client.Subscriptions.Commands.Subscribe;

public class SubscribeCommand : IRequest<SubscribeResponse>
{
    public long SubscriptionTypeId { get; set; }
}

public class SubscribeResponse
{
    public long SubscriptionId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
