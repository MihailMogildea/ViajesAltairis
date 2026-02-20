namespace ViajesAltairis.Application.Features.Admin.SubscriptionTypes.Dtos;

public record CreateSubscriptionTypeRequest(string Name, decimal PricePerMonth, decimal Discount, long CurrencyId);
