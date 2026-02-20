using System.Text.Json.Serialization;
using MediatR;

namespace ViajesAltairis.Application.Features.Client.Hotels.Queries.GetRoomAvailability;

public class GetRoomAvailabilityQuery : IRequest<GetRoomAvailabilityResponse>
{
    public long HotelId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int Guests { get; set; }
}

public class GetRoomAvailabilityResponse
{
    public List<RoomAvailabilityDto> Rooms { get; set; } = new();
    public string CurrencyCode { get; set; } = "EUR";
}

public class RoomAvailabilityDto
{
    public long RoomTypeId { get; set; }
    public long RoomTypeDbId { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public int MaxGuests { get; set; }
    public int AvailableRooms { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    [JsonIgnore] public long ProviderTypeId { get; set; }
    [JsonIgnore] public long ProviderId { get; set; }
    public List<BoardOptionDto> BoardOptions { get; set; } = new();
}

public class BoardOptionDto
{
    public long BoardTypeId { get; set; }
    public string BoardTypeName { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
}
