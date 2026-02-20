namespace ViajesAltairis.Application.Features.ExternalClient.Hotels.Dtos;

public record RoomAvailabilityDto(
    long HotelProviderRoomTypeId,
    string RoomTypeName,
    string ProviderName,
    int TotalRooms,
    decimal PricePerNight,
    string CurrencyCode,
    int AvailableRooms);
