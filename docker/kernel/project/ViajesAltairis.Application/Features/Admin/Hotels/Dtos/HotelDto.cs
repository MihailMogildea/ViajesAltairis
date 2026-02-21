namespace ViajesAltairis.Application.Features.Admin.Hotels.Dtos;

public class HotelDto
{
    public long Id { get; init; }
    public long CityId { get; init; }
    public string Name { get; init; } = null!;
    public int Stars { get; init; }
    public string Address { get; init; } = null!;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public TimeOnly CheckInTime { get; init; }
    public TimeOnly CheckOutTime { get; init; }
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
    public decimal Margin { get; init; }
    public bool Enabled { get; init; }
    public DateTime CreatedAt { get; init; }
}
