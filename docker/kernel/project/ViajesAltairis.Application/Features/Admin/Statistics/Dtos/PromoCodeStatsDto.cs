namespace ViajesAltairis.Application.Features.Admin.Statistics.Dtos;

public class PromoCodeStatsDto
{
    public long PromoCodeId { get; init; }
    public string Code { get; init; } = null!;
    public int UsageCount { get; init; }
    public decimal TotalDiscount { get; init; }
    public string CurrencyCode { get; init; } = null!;
}
