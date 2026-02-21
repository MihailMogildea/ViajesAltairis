namespace ViajesAltairis.Application.Features.Admin.PaymentTransactions.Dtos;

public record PaymentTransactionDto(
    long Id,
    long ReservationId,
    long PaymentMethodId,
    string TransactionReference,
    decimal Amount,
    long CurrencyId,
    string CurrencyCode,
    long ExchangeRateId,
    long StatusId,
    DateTime CreatedAt,
    DateTime UpdatedAt);
