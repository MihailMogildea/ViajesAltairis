namespace ViajesAltairis.Application.Features.ExternalClient.Hotels.Dtos;

public class HotelCardDto
{
    public long HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public int Stars { get; set; }
    public long CityId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string AdminDivisionName { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public decimal? AvgRating { get; set; }
    public int ReviewCount { get; set; }
    public int? FreeCancellationHours { get; set; }
}
