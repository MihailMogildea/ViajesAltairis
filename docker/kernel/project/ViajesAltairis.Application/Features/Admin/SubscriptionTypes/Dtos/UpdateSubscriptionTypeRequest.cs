namespace ViajesAltairis.Application.Features.Admin.SubscriptionTypes.Dtos;

public record UpdateSubscriptionTypeRequest(string Name, decimal PricePerMonth, decimal Discount, long CurrencyId);
