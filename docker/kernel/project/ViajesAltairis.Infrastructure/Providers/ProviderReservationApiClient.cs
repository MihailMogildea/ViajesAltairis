using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Infrastructure.Providers;

public class ProviderReservationApiClient : IProviderReservationService
{
    private readonly HttpClient _httpClient;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<ProviderReservationApiClient> _logger;

    public ProviderReservationApiClient(
        HttpClient httpClient,
        IDbConnectionFactory connectionFactory,
        ILogger<ProviderReservationApiClient> logger)
    {
        _httpClient = httpClient;
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<ProviderBookingResult> CreateBookingAsync(ProviderBookingRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating booking with provider {ProviderId} for hotel {HotelId}, room config {RoomConfigId}...",
            request.ProviderId, request.HotelId, request.RoomConfigurationId);

        // Resolve IDs to names for the providers-api BookingRequest
        using var connection = _connectionFactory.CreateConnection();
        var details = await Dapper.SqlMapper.QuerySingleOrDefaultAsync<dynamic>(
            connection,
            """
            SELECT h.name AS hotel_name, rt.name AS room_type, bt.name AS board_type
            FROM hotel_provider_room_type hprt
            JOIN hotel_provider hp ON hp.id = hprt.hotel_provider_id
            JOIN hotel h ON h.id = hp.hotel_id
            JOIN room_type rt ON rt.id = hprt.room_type_id
            JOIN board_type bt ON bt.id = (
                SELECT board_type_id FROM hotel_provider_room_type_board
                WHERE hotel_provider_room_type_id = hprt.id LIMIT 1
            )
            WHERE hprt.id = @RoomConfigId
            """,
            new { RoomConfigId = request.RoomConfigurationId });

        if (details is null)
        {
            _logger.LogWarning("Could not resolve room configuration {RoomConfigId} for provider booking", request.RoomConfigurationId);
            return new ProviderBookingResult(false, FailureReason: "Room configuration not found");
        }

        var bookingRequest = new
        {
            HotelName = (string)details.hotel_name,
            RoomType = (string)details.room_type,
            BoardType = (string)details.board_type,
            CheckIn = request.CheckIn,
            CheckOut = request.CheckOut,
            Guests = request.GuestCount,
            GuestName = "Reservation Guest",
            GuestEmail = ""
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"api/providers/{request.ProviderId}/book", bookingRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Provider booking failed with status {Status}: {Error}",
                response.StatusCode, errorBody);
            return new ProviderBookingResult(false, FailureReason: $"Provider returned {response.StatusCode}");
        }

        var result = await response.Content.ReadFromJsonAsync<ExternalBookingResponse>(cancellationToken);

        if (result is null || !result.Success)
        {
            _logger.LogWarning("Provider booking returned failure: {Error}", result?.Error);
            return new ProviderBookingResult(false, FailureReason: result?.Error ?? "Unknown error");
        }

        _logger.LogInformation("Provider booking created: {Reference}", result.BookingReference);
        return new ProviderBookingResult(true, result.BookingReference);
    }

    public async Task<bool> CancelBookingAsync(long providerId, string externalReference, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Cancelling booking with provider {ProviderId}, reference {Reference}...",
            providerId, externalReference);

        var response = await _httpClient.DeleteAsync(
            $"api/providers/{providerId}/bookings/{Uri.EscapeDataString(externalReference)}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Provider cancellation failed with status {Status}", response.StatusCode);
            return false;
        }

        _logger.LogInformation("Provider booking cancelled: {Reference}", externalReference);
        return true;
    }

    // Matches the ExternalBookingResult shape from providers-api
    private record ExternalBookingResponse(bool Success, string? BookingReference, string? Error);
}
