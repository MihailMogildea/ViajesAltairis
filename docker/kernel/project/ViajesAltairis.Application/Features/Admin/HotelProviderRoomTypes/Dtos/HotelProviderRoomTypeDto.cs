namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypes.Dtos;

public class HotelProviderRoomTypeDto
{
    public long Id { get; init; }
    public long HotelProviderId { get; init; }
    public long RoomTypeId { get; init; }
    public int Capacity { get; init; }
    public int Quantity { get; init; }
    public decimal PricePerNight { get; init; }
    public long CurrencyId { get; init; }
    public long ExchangeRateId { get; init; }
    public bool Enabled { get; init; }
    public DateTime CreatedAt { get; init; }
}
