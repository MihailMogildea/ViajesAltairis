namespace ViajesAltairis.Application.Features.Admin.PromoCodes.Dtos;

public class PromoCodeDto
{
    public long Id { get; init; }
    public string Code { get; init; } = null!;
    public decimal? DiscountPercentage { get; init; }
    public decimal? DiscountAmount { get; init; }
    public long? CurrencyId { get; init; }
    public DateOnly ValidFrom { get; init; }
    public DateOnly ValidTo { get; init; }
    public int? MaxUses { get; init; }
    public int CurrentUses { get; init; }
    public bool Enabled { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
