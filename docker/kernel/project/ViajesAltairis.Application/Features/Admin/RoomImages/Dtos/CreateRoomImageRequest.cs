namespace ViajesAltairis.Application.Features.Admin.RoomImages.Dtos;

public record CreateRoomImageRequest(long HotelProviderRoomTypeId, string Url, string? AltText, int SortOrder);
