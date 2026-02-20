namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeBoards.Dtos;

public record HotelProviderRoomTypeBoardDto(long Id, long HotelProviderRoomTypeId, long BoardTypeId, decimal PricePerNight, bool Enabled);
