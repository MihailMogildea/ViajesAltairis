namespace ViajesAltairis.Application.Features.Admin.PaymentTransactions.Dtos;

public record PaymentTransactionDto(
    long Id,
    long ReservationId,
    long PaymentMethodId,
    string TransactionReference,
    decimal Amount,
    long CurrencyId,
    long ExchangeRateId,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt);
