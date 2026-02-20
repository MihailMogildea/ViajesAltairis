namespace ViajesAltairis.Application.Features.Admin.UserSubscriptions.Dtos;

public record UserSubscriptionDto(long Id, long UserId, long SubscriptionTypeId, DateOnly StartDate, DateOnly? EndDate, bool Active, DateTime CreatedAt, DateTime UpdatedAt);
