using MediatR;

namespace ViajesAltairis.Application.Features.Client.PromoCodes.Queries.ValidatePromoCode;

public class ValidatePromoCodeQuery : IRequest<ValidatePromoCodeResponse>
{
    public string Code { get; set; } = string.Empty;
}

public class ValidatePromoCodeResponse
{
    public bool IsValid { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal? DiscountAmount { get; set; }
    public string? CurrencyCode { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? Message { get; set; }
}
