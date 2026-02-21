namespace ViajesAltairis.Application.Features.ExternalClient.Hotels.Dtos;

public record HotelDetailDto(
    long HotelId, string HotelName, int Stars, string Address,
    string? Email, string? Phone, TimeOnly CheckInTime, TimeOnly CheckOutTime,
    decimal? Latitude, decimal? Longitude, string CityName, string AdminDivisionName, string CountryName,
    decimal? AvgRating, int ReviewCount, int? FreeCancellationHours, decimal? PenaltyPercentage,
    List<RoomCatalogDto> Rooms, List<HotelAmenityDto> Amenities);

public record RoomCatalogDto(
    long HotelProviderRoomTypeId, string RoomTypeName, string ProviderName,
    int Capacity, int Quantity, decimal PricePerNight, string CurrencyCode, bool Enabled,
    List<BoardOptionDto> BoardOptions);

public record BoardOptionDto(long HotelProviderRoomTypeBoardId, string BoardTypeName, decimal PricePerNight, bool Enabled);

public class HotelAmenityDto
{
    public string AmenityName { get; set; } = string.Empty;
    public string AmenityCategoryName { get; set; } = string.Empty;
}
