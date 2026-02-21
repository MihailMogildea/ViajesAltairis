namespace ViajesAltairis.Application.Features.Admin.PaymentTransactionFees.Dtos;

public class PaymentTransactionFeeDto
{
    public long Id { get; init; }
    public long PaymentTransactionId { get; init; }
    public string FeeType { get; init; } = null!;
    public decimal FeeAmount { get; init; }
    public decimal? FeePercentage { get; init; }
    public decimal? FixedFeeAmount { get; init; }
    public long CurrencyId { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
}
