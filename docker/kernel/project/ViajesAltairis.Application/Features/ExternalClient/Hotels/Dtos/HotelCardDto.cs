namespace ViajesAltairis.Application.Features.ExternalClient.Hotels.Dtos;

public record HotelCardDto(
    long HotelId,
    string HotelName,
    int Stars,
    long CityId,
    string CityName,
    string AdminDivisionName,
    string CountryName,
    decimal? AvgRating,
    int ReviewCount,
    int? FreeCancellationHours);
