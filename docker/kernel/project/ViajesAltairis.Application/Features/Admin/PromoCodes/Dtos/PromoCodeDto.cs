namespace ViajesAltairis.Application.Features.Admin.PromoCodes.Dtos;

public record PromoCodeDto(long Id, string Code, decimal? DiscountPercentage, decimal? DiscountAmount, long? CurrencyId, DateOnly ValidFrom, DateOnly ValidTo, int? MaxUses, int CurrentUses, bool Enabled, DateTime CreatedAt, DateTime UpdatedAt);
