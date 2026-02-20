namespace ViajesAltairis.Application.Features.Admin.HotelImages.Dtos;

public record UpdateHotelImageRequest(long HotelId, string Url, string? AltText, int SortOrder);
