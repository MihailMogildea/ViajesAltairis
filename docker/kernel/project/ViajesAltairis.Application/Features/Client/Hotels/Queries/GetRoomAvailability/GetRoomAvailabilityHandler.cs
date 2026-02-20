using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Hotels.Queries.GetRoomAvailability;

public class GetRoomAvailabilityHandler : IRequestHandler<GetRoomAvailabilityQuery, GetRoomAvailabilityResponse>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ITranslationService _translationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICurrencyConverter _currencyConverter;
    private readonly ICacheService _cacheService;
    private readonly IProvidersApiClient _providersApiClient;
    private readonly ILogger<GetRoomAvailabilityHandler> _logger;

    public GetRoomAvailabilityHandler(
        IDbConnectionFactory connectionFactory,
        ITranslationService translationService,
        ICurrentUserService currentUserService,
        ICurrencyConverter currencyConverter,
        ICacheService cacheService,
        IProvidersApiClient providerAvailability,
        ILogger<GetRoomAvailabilityHandler> logger)
    {
        _connectionFactory = connectionFactory;
        _translationService = translationService;
        _currentUserService = currentUserService;
        _currencyConverter = currencyConverter;
        _cacheService = cacheService;
        _providersApiClient = providerAvailability;
        _logger = logger;
    }

    public async Task<GetRoomAvailabilityResponse> Handle(
        GetRoomAvailabilityQuery request,
        CancellationToken cancellationToken)
    {
        var langId = _currentUserService.LanguageId;
        var currency = _currentUserService.CurrencyCode;
        var cacheKey = $"hotel:rooms:{request.HotelId}:{request.CheckIn:yyyy-MM-dd}:{request.CheckOut:yyyy-MM-dd}:{request.Guests}:{langId}:{currency}";

        var cached = await _cacheService.GetAsync<GetRoomAvailabilityResponse>(cacheKey, cancellationToken);
        if (cached != null) return cached;

        using var connection = _connectionFactory.CreateConnection();

        const string roomsSql = """
            SELECT
                rc.hotel_provider_room_type_id AS RoomTypeId,
                rc.room_type_id AS RoomTypeDbId,
                rc.room_type_name AS RoomTypeName,
                rc.capacity AS MaxGuests,
                rc.quantity AS AvailableRooms,
                rc.currency_code AS CurrencyCode,
                rc.provider_type_id AS ProviderTypeId,
                rc.provider_id AS ProviderId
            FROM v_hotel_room_catalog rc
            WHERE rc.hotel_id = @HotelId
              AND rc.enabled = TRUE
              AND rc.capacity >= @Guests
            ORDER BY rc.room_type_name
            """;

        var rooms = (await connection.QueryAsync<RoomAvailabilityDto>(
            roomsSql,
            new { request.HotelId, request.Guests })).ToList();

        var hasExternalProviders = rooms.Any(r => r.ProviderTypeId == 2);

        if (rooms.Count > 0)
        {
            var roomIds = rooms.Select(r => r.RoomTypeId).ToList();

            const string boardsSql = """
                SELECT
                    hotel_provider_room_type_id AS RoomTypeId,
                    board_type_id AS BoardTypeId,
                    board_type_name AS BoardTypeName,
                    price_per_night AS PricePerNight
                FROM v_room_board_option
                WHERE hotel_provider_room_type_id IN @RoomIds
                  AND enabled = TRUE
                ORDER BY price_per_night
                """;

            var boards = await connection.QueryAsync<BoardOptionRow>(
                boardsSql, new { RoomIds = roomIds });
            var boardsByRoom = boards
                .GroupBy(b => b.RoomTypeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var room in rooms)
            {
                room.BoardOptions = boardsByRoom.TryGetValue(room.RoomTypeId, out var roomBoards)
                    ? roomBoards.Select(b => new BoardOptionDto
                    {
                        BoardTypeId = b.BoardTypeId,
                        BoardTypeName = b.BoardTypeName,
                        PricePerNight = b.PricePerNight
                    }).ToList()
                    : [];
            }

            // Overlay live availability for external provider rooms
            if (hasExternalProviders)
            {
                var externalGroups = rooms
                    .Where(r => r.ProviderTypeId == 2)
                    .GroupBy(r => r.ProviderId)
                    .ToList();

                var fetchTasks = externalGroups.Select(g =>
                    SafeFetchAvailabilityAsync(
                        g.Key, request.HotelId,
                        request.CheckIn, request.CheckOut,
                        request.Guests, cancellationToken));

                var fetchResults = await Task.WhenAll(fetchTasks);

                foreach (var (group, availability) in externalGroups.Zip(fetchResults))
                {
                    if (availability is null) continue;

                    foreach (var room in group)
                    {
                        var match = availability.FirstOrDefault(
                            r => r.RoomType.Equals(
                                room.RoomTypeName,
                                StringComparison.OrdinalIgnoreCase));
                        if (match is null) continue;

                        room.AvailableRooms = match.Available;
                        foreach (var board in room.BoardOptions)
                        {
                            var liveBoard = match.Boards.FirstOrDefault(
                                b => b.BoardType.Equals(
                                    board.BoardTypeName,
                                    StringComparison.OrdinalIgnoreCase));
                            if (liveBoard is not null)
                                board.PricePerNight = liveBoard.PricePerNight;
                        }
                    }
                }
            }

            // Translations
            var hprtSummaries = await _translationService.ResolveAsync(
                "hotel_provider_room_type", roomIds, langId, "summary", cancellationToken);
            foreach (var room in rooms)
                if (hprtSummaries.TryGetValue(room.RoomTypeId, out var s))
                    room.Summary = s;

            if (langId != 1)
            {
                var rtIds = rooms.Select(r => r.RoomTypeDbId).Distinct().ToList();
                var rtNames = await _translationService.ResolveAsync(
                    "room_type", rtIds, langId, "name", cancellationToken);
                foreach (var room in rooms)
                    if (rtNames.TryGetValue(room.RoomTypeDbId, out var n))
                        room.RoomTypeName = n;

                var btIds = rooms.SelectMany(r => r.BoardOptions)
                    .Select(b => b.BoardTypeId).Distinct().ToList();
                if (btIds.Count > 0)
                {
                    var btNames = await _translationService.ResolveAsync(
                        "board_type", btIds, langId, "name", cancellationToken);
                    foreach (var room in rooms)
                        foreach (var bo in room.BoardOptions)
                            if (btNames.TryGetValue(bo.BoardTypeId, out var bn))
                                bo.BoardTypeName = bn;
                }
            }
        }

        // Currency conversion
        var targetCurrency = _currentUserService.CurrencyCode;
        if (rooms.Count > 0)
        {
            var sourceCurrencyCode = rooms.First().CurrencyCode;
            if (!string.IsNullOrEmpty(sourceCurrencyCode)
                && sourceCurrencyCode != targetCurrency)
            {
                var currencyIds = await connection.QueryAsync<(long Id, string IsoCode)>(
                    "SELECT id, iso_code FROM currency WHERE iso_code IN @Codes",
                    new { Codes = new[] { sourceCurrencyCode, targetCurrency } });
                var lookup = currencyIds.ToDictionary(c => c.IsoCode, c => c.Id);

                if (lookup.TryGetValue(sourceCurrencyCode, out var sourceId)
                    && lookup.TryGetValue(targetCurrency, out var targetId))
                {
                    var (factor, _) = await _currencyConverter.ConvertAsync(
                        1m, sourceId, targetId, cancellationToken);
                    foreach (var room in rooms)
                        foreach (var bo in room.BoardOptions)
                            bo.PricePerNight = Math.Round(bo.PricePerNight * factor, 2);
                }
            }
        }

        var response = new GetRoomAvailabilityResponse
        {
            Rooms = rooms, CurrencyCode = targetCurrency
        };
        var cacheTtl = hasExternalProviders
            ? TimeSpan.FromMinutes(2)
            : TimeSpan.FromMinutes(10);
        await _cacheService.SetAsync(cacheKey, response, cacheTtl, cancellationToken);
        return response;
    }

    private async Task<List<ExternalRoomAvailability>?> SafeFetchAvailabilityAsync(
        long providerId, long hotelId,
        DateTime checkIn, DateTime checkOut,
        int guests, CancellationToken ct)
    {
        try
        {
            return await _providersApiClient.GetHotelAvailabilityAsync(
                providerId, hotelId, checkIn, checkOut, guests, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Failed to fetch availability from provider {ProviderId} for hotel {HotelId}. Falling back to DB data.",
                providerId, hotelId);
            return null;
        }
    }

    private record BoardOptionRow(
        long RoomTypeId, long BoardTypeId,
        string BoardTypeName, decimal PricePerNight);
}
