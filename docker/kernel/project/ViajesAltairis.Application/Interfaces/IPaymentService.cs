namespace ViajesAltairis.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request, CancellationToken cancellationToken = default);
    Task<RefundResult> ProcessRefundAsync(long reservationId, string paymentReference, CancellationToken cancellationToken = default);
}

public record PaymentRequest(
    long ReservationId,
    long PaymentMethodId,
    decimal Amount,
    string CurrencyCode,
    string? CardNumber,
    string? CardExpiry,
    string? CardCvv,
    string? CardHolderName);

public record PaymentResult(bool Success, string? PaymentReference = null, string? FailureReason = null);
public record RefundResult(bool Success, string? RefundReference = null, string? FailureReason = null);
