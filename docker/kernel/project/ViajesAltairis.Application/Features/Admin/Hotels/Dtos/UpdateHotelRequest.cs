namespace ViajesAltairis.Application.Features.Admin.Hotels.Dtos;

public record UpdateHotelRequest(long CityId, string Name, byte Stars, string Address, string? Email, string? Phone, string CheckInTime, string CheckOutTime, decimal? Latitude, decimal? Longitude, decimal Margin);
