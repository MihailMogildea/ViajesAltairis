namespace ViajesAltairis.Application.Interfaces;

public interface IProviderReservationService
{
    /// <summary>
    /// Creates a reservation with an external provider via providers-api.
    /// </summary>
    Task<ProviderBookingResult> CreateBookingAsync(ProviderBookingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels a previously confirmed booking with an external provider.
    /// </summary>
    Task<bool> CancelBookingAsync(long providerId, string externalReference, CancellationToken cancellationToken = default);
}

public record ProviderBookingRequest(
    long ProviderId,
    long HotelId,
    long RoomConfigurationId,
    DateTime CheckIn,
    DateTime CheckOut,
    int GuestCount);

public record ProviderBookingResult(bool Success, string? ExternalReference = null, string? FailureReason = null);
