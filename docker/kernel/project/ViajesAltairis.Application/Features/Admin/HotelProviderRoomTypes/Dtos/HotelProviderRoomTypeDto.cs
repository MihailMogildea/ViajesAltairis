namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypes.Dtos;

public record HotelProviderRoomTypeDto(long Id, long HotelProviderId, long RoomTypeId, byte Capacity, int Quantity, decimal PricePerNight, long CurrencyId, long ExchangeRateId, bool Enabled, DateTime CreatedAt);
