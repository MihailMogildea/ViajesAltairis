namespace ViajesAltairis.Application.Features.Admin.PaymentTransactionFees.Dtos;

public record PaymentTransactionFeeDto(
    long Id,
    long PaymentTransactionId,
    string FeeType,
    decimal FeeAmount,
    decimal? FeePercentage,
    decimal? FixedFeeAmount,
    long CurrencyId,
    string? Description,
    DateTime CreatedAt);
