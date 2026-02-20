namespace ViajesAltairis.ProvidersApi.ExternalClients;

public interface IProviderResponseMapper<TRawHotels, TRawAvailability, TRawBooking>
{
    IEnumerable<ExternalHotel> MapHotels(TRawHotels raw);
    ExternalAvailabilityResponse MapAvailability(TRawAvailability raw);
    ExternalBookingResult MapBooking(TRawBooking raw);
}
