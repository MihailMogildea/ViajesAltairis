namespace ViajesAltairis.Application.Features.Admin.SubscriptionTypes.Dtos;

public record SubscriptionTypeDto(long Id, string Name, decimal PricePerMonth, decimal Discount, long CurrencyId, bool Enabled, DateTime CreatedAt, DateTime UpdatedAt);
