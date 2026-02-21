namespace ViajesAltairis.Application.Features.Admin.Hotels.Dtos;

public record CreateHotelRequest(long CityId, string Name, int Stars, string Address, string? Email, string? Phone, string CheckInTime, string CheckOutTime, decimal? Latitude, decimal? Longitude, decimal Margin);
