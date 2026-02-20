namespace ViajesAltairis.Application.Features.Admin.Cancellations.Dtos;

public record CancellationDto(
    long Id,
    long ReservationId,
    long CancelledByUserId,
    string? Reason,
    decimal PenaltyPercentage,
    decimal PenaltyAmount,
    decimal RefundAmount,
    long CurrencyId,
    DateTime CreatedAt);
