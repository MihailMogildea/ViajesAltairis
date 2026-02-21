namespace ViajesAltairis.Application.Features.ExternalClient.Hotels.Dtos;

public class RoomAvailabilityDto
{
    public long HotelProviderRoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public int TotalRooms { get; set; }
    public decimal PricePerNight { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public int AvailableRooms { get; set; }
}
