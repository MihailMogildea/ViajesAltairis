namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeBoards.Dtos;

public record CreateHotelProviderRoomTypeBoardRequest(long HotelProviderRoomTypeId, long BoardTypeId, decimal PricePerNight, bool Enabled);
