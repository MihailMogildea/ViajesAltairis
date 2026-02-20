namespace ViajesAltairis.Application.Features.Admin.HotelImages.Dtos;

public record CreateHotelImageRequest(long HotelId, string Url, string? AltText, int SortOrder);
