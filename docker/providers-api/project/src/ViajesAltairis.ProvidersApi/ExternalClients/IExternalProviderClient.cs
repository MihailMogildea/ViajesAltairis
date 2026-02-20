namespace ViajesAltairis.ProvidersApi.ExternalClients;

public interface IExternalProviderClient
{
    string ProviderName { get; }
    Task<IEnumerable<ExternalHotel>> GetHotelsAsync();
    Task<ExternalAvailabilityResponse> SearchAvailabilityAsync(AvailabilityRequest request);
    Task<ExternalBookingResult> BookAsync(BookingRequest request);
    Task<ExternalCancellationResult> CancelAsync(string bookingReference);
}
