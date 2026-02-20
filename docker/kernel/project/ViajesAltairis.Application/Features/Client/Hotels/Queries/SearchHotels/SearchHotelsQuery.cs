using MediatR;

namespace ViajesAltairis.Application.Features.Client.Hotels.Queries.SearchHotels;

public class SearchHotelsQuery : IRequest<SearchHotelsResponse>
{
    public long? CityId { get; set; }
    public long? CountryId { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public int? Guests { get; set; }
    public int? Stars { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class SearchHotelsResponse
{
    public List<HotelSummaryDto> Hotels { get; set; } = new();
    public int TotalCount { get; set; }
    public string CurrencyCode { get; set; } = "EUR";
}

public class HotelSummaryDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public int Stars { get; set; }
    public long CityId { get; set; }
    public string City { get; set; } = string.Empty;
    public long CountryId { get; set; }
    public string Country { get; set; } = string.Empty;
    public decimal AvgRating { get; set; }
    public int ReviewCount { get; set; }
    public decimal? PriceFrom { get; set; }
    public string? MainImageUrl { get; set; }
}
