using System.Net.Http.Json;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Infrastructure.Services;

public class ProvidersApiClient : IProvidersApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ProvidersApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient() => _httpClientFactory.CreateClient("ProvidersApi");

    public async Task<List<ExternalRoomAvailability>> GetHotelAvailabilityAsync(
        long providerId, long hotelId,
        DateTime checkIn, DateTime checkOut, int guests,
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var url = $"/api/providers/{providerId}/availability/hotel/{hotelId}?checkIn={checkIn:yyyy-MM-dd}&checkOut={checkOut:yyyy-MM-dd}&guests={guests}";
        var response = await client.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<HotelAvailabilityResponse>(cancellationToken: cancellationToken);
        return result?.Rooms?.Select(r => new ExternalRoomAvailability(
            r.RoomType, r.Capacity, r.PricePerNight, r.Available,
            r.Boards.Select(b => new ExternalBoardOption(b.BoardType, b.PricePerNight)).ToList()
        )).ToList() ?? [];
    }

    private record HotelAvailabilityResponse(List<AvailableRoomDto> Rooms);
    private record AvailableRoomDto(string RoomType, int Capacity, decimal PricePerNight, int Available, List<AvailableBoardDto> Boards);
    private record AvailableBoardDto(string BoardType, decimal PricePerNight);
}
