using MediatR;

namespace ViajesAltairis.Application.Features.Client.Hotels.Queries.GetHotelDetail;

public class GetHotelDetailQuery : IRequest<GetHotelDetailResponse>
{
    public long HotelId { get; set; }
}

public class GetHotelDetailResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public int Stars { get; set; }
    public string Address { get; set; } = string.Empty;
    public long CityId { get; set; }
    public string City { get; set; } = string.Empty;
    public long CountryId { get; set; }
    public string Country { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? CheckInTime { get; set; }
    public string? CheckOutTime { get; set; }
    public decimal AvgRating { get; set; }
    public int ReviewCount { get; set; }
    public int? FreeCancellationHours { get; set; }
    public List<string> Images { get; set; } = new();
    public List<AmenityDto> Amenities { get; set; } = new();
}

public class AmenityDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
