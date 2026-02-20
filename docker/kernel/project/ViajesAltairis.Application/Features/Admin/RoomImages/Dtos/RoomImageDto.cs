namespace ViajesAltairis.Application.Features.Admin.RoomImages.Dtos;

public record RoomImageDto(long Id, long HotelProviderRoomTypeId, string Url, string? AltText, int SortOrder, DateTime CreatedAt);
