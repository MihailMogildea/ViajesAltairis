using Microsoft.Extensions.Logging;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Infrastructure.Payment;

public class PaymentService : IPaymentService
{
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(ILogger<PaymentService> logger)
    {
        _logger = logger;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing payment for reservation {ReservationId} via payment method {MethodId}...",
            request.ReservationId, request.PaymentMethodId);

        // Simulate processing delay based on payment method
        switch (request.PaymentMethodId)
        {
            case 1: // Visa
            case 2: // Mastercard
                await Task.Delay(2000, cancellationToken);
                break;
            case 4: // PayPal
                await Task.Delay(1500, cancellationToken);
                break;
            case 3: // Bank Transfer
                _logger.LogInformation("Bank transfer for reservation {ReservationId} — status pending, confirmed later by scheduled job",
                    request.ReservationId);
                return new PaymentResult(true, $"PAY-{Guid.NewGuid():N}");
            default:
                return new PaymentResult(false, FailureReason: $"Unknown payment method {request.PaymentMethodId}");
        }

        var reference = $"PAY-{Guid.NewGuid():N}";
        _logger.LogInformation("Payment completed for reservation {ReservationId}: {Reference}",
            request.ReservationId, reference);

        return new PaymentResult(true, reference);
    }

    public async Task<RefundResult> ProcessRefundAsync(long reservationId, string paymentReference, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing refund for reservation {ReservationId}, payment {PaymentReference}...",
            reservationId, paymentReference);

        await Task.Delay(1000, cancellationToken);

        var reference = $"REF-{Guid.NewGuid():N}";
        _logger.LogInformation("Refund completed for reservation {ReservationId}: {Reference}",
            reservationId, reference);

        return new RefundResult(true, reference);
    }
}
