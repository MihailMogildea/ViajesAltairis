namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeBoards.Dtos;

public record UpdateHotelProviderRoomTypeBoardRequest(long HotelProviderRoomTypeId, long BoardTypeId, decimal PricePerNight, bool Enabled);
