namespace ViajesAltairis.Application.Features.Admin.UserSubscriptions.Dtos;

public class UserSubscriptionDto
{
    public long Id { get; init; }
    public long UserId { get; init; }
    public string UserEmail { get; init; } = null!;
    public long SubscriptionTypeId { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public bool Active { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
