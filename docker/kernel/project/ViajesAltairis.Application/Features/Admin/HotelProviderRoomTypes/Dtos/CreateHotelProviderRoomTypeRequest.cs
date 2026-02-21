namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypes.Dtos;

public record CreateHotelProviderRoomTypeRequest(long HotelProviderId, long RoomTypeId, int Capacity, int Quantity, decimal PricePerNight, long CurrencyId, long ExchangeRateId);
