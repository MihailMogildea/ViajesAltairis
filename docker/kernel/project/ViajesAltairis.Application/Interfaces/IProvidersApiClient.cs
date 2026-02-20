namespace ViajesAltairis.Application.Interfaces;

public interface IProvidersApiClient
{
    Task<List<ExternalRoomAvailability>> GetHotelAvailabilityAsync(
        long providerId, long hotelId,
        DateTime checkIn, DateTime checkOut, int guests,
        CancellationToken cancellationToken = default);
}

public record ExternalRoomAvailability(
    string RoomType, int Capacity, decimal PricePerNight,
    int Available, List<ExternalBoardOption> Boards);

public record ExternalBoardOption(string BoardType, decimal PricePerNight);
