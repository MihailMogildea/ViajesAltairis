namespace ViajesAltairis.Application.Features.Admin.RoomImages.Dtos;

public record UpdateRoomImageRequest(long HotelProviderRoomTypeId, string Url, string? AltText, int SortOrder);
