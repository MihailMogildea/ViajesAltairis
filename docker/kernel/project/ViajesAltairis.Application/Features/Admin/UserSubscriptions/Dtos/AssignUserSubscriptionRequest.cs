namespace ViajesAltairis.Application.Features.Admin.UserSubscriptions.Dtos;

public record AssignUserSubscriptionRequest(long UserId, long SubscriptionTypeId, DateOnly StartDate, DateOnly? EndDate);
