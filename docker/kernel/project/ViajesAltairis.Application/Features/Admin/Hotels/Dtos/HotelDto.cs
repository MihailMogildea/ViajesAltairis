namespace ViajesAltairis.Application.Features.Admin.Hotels.Dtos;

public record HotelDto(long Id, long CityId, string Name, byte Stars, string Address, string? Email, string? Phone, TimeOnly CheckInTime, TimeOnly CheckOutTime, decimal? Latitude, decimal? Longitude, decimal Margin, bool Enabled, DateTime CreatedAt);
