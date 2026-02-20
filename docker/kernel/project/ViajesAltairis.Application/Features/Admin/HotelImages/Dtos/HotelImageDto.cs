namespace ViajesAltairis.Application.Features.Admin.HotelImages.Dtos;

public record HotelImageDto(long Id, long HotelId, string Url, string? AltText, int SortOrder, DateTime CreatedAt);
