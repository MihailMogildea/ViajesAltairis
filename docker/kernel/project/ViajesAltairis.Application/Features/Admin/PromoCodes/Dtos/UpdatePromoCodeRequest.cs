namespace ViajesAltairis.Application.Features.Admin.PromoCodes.Dtos;

public record UpdatePromoCodeRequest(string Code, decimal? DiscountPercentage, decimal? DiscountAmount, long? CurrencyId, DateOnly ValidFrom, DateOnly ValidTo, int? MaxUses);
