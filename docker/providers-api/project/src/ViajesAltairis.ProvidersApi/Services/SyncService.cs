using StackExchange.Redis;
using ViajesAltairis.ProvidersApi.ExternalClients;
using ViajesAltairis.ProvidersApi.Repositories;

namespace ViajesAltairis.ProvidersApi.Services;

public class SyncService
{
    private readonly IEnumerable<IExternalProviderClient> _clients;
    private readonly IProviderRepository _providerRepo;
    private readonly IHotelSyncRepository _hotelSyncRepo;
    private readonly ILogger<SyncService> _logger;
    private readonly IConnectionMultiplexer _redis;
    private static readonly Random Rng = new();

    public SyncService(
        IEnumerable<IExternalProviderClient> clients,
        IProviderRepository providerRepo,
        IHotelSyncRepository hotelSyncRepo,
        ILogger<SyncService> logger,
        IConnectionMultiplexer redis)
    {
        _clients = clients;
        _providerRepo = providerRepo;
        _hotelSyncRepo = hotelSyncRepo;
        _logger = logger;
        _redis = redis;
    }

    public IExternalProviderClient? GetClientForProvider(long providerId, string providerName)
    {
        return _clients.FirstOrDefault(c => c.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> TryStartSyncAsync(long providerId)
    {
        return await _providerRepo.TrySetSyncStatusAsync(providerId, "updating", null);
    }

    public async Task ExecuteSyncAsync(long providerId, IExternalProviderClient client)
    {
        try
        {
            _logger.LogInformation("Starting sync for provider {ProviderId} ({ProviderName})", providerId, client.ProviderName);

            // Look up the provider's currency and current exchange rate
            var provider = await _providerRepo.GetByIdAsync(providerId);
            string currencyCode = (string)provider!.currency_iso_code;

            var currencyId = await _hotelSyncRepo.GetCurrencyIdByCodeAsync(currencyCode);
            if (currencyId == null)
            {
                _logger.LogError("Currency '{CurrencyCode}' not found in database, aborting sync for provider {ProviderId}", currencyCode, providerId);
                await _providerRepo.SetSyncFailedAsync(providerId);
                return;
            }

            var exchangeRateId = await _hotelSyncRepo.GetCurrentExchangeRateIdAsync(currencyId.Value);
            if (exchangeRateId == null)
            {
                _logger.LogError("No current exchange rate for {CurrencyCode} found, aborting sync for provider {ProviderId}", currencyCode, providerId);
                await _providerRepo.SetSyncFailedAsync(providerId);
                return;
            }

            _logger.LogInformation("Provider {ProviderId} uses currency {CurrencyCode} (id={CurrencyId}, exchangeRateId={ExchangeRateId})",
                providerId, currencyCode, currencyId, exchangeRateId);

            var hotels = await client.GetHotelsAsync();
            var count = 0;

            foreach (var hotel in hotels)
            {
                var cityId = await _hotelSyncRepo.GetCityIdByNameAsync(hotel.CityName);
                if (cityId == null)
                {
                    _logger.LogWarning("City '{CityName}' not found, skipping hotel '{HotelName}'", hotel.CityName, hotel.Name);
                    continue;
                }

                // Match or create hotel
                var existing = await _hotelSyncRepo.FindHotelAsync(hotel.Name, hotel.CityName);
                long hotelId;
                if (existing != null)
                {
                    hotelId = (long)existing.id;
                    _logger.LogDebug("Matched existing hotel '{HotelName}' (id={HotelId})", hotel.Name, hotelId);
                }
                else
                {
                    hotelId = await _hotelSyncRepo.InsertHotelAsync(cityId.Value, hotel.Name, hotel.Stars, hotel.Address, hotel.Email, hotel.Phone);
                    _logger.LogInformation("Created new hotel '{HotelName}' in {CityName} (id={HotelId})", hotel.Name, hotel.CityName, hotelId);
                }

                // Ensure hotel-provider link
                var hpId = await _hotelSyncRepo.FindHotelProviderAsync(hotelId, providerId);
                if (hpId == null)
                {
                    hpId = await _hotelSyncRepo.InsertHotelProviderAsync(hotelId, providerId);
                    _logger.LogDebug("Created hotel-provider link (id={HpId})", hpId);
                }

                // Upsert rooms and boards
                foreach (var room in hotel.Rooms)
                {
                    var roomTypeId = await _hotelSyncRepo.GetRoomTypeIdAsync(room.RoomTypeName);
                    if (roomTypeId == null)
                    {
                        _logger.LogWarning("Room type '{RoomType}' not found, skipping", room.RoomTypeName);
                        continue;
                    }

                    // Apply ±15% random price variation for demo
                    var priceVariation = 1.0m + (decimal)(Rng.NextDouble() * 0.30 - 0.15);
                    var adjustedPrice = Math.Round(room.PricePerNight * priceVariation, 2);

                    var hprtId = await _hotelSyncRepo.UpsertHotelProviderRoomTypeAsync(
                        hpId.Value, roomTypeId.Value, room.Capacity, room.Quantity, adjustedPrice, currencyId.Value, exchangeRateId.Value);

                    foreach (var board in room.Boards)
                    {
                        var boardTypeId = await _hotelSyncRepo.GetBoardTypeIdAsync(board.BoardTypeName);
                        if (boardTypeId == null)
                        {
                            _logger.LogWarning("Board type '{BoardType}' not found, skipping", board.BoardTypeName);
                            continue;
                        }

                        var boardPriceVariation = 1.0m + (decimal)(Rng.NextDouble() * 0.30 - 0.15);
                        var adjustedBoardPrice = Math.Round(board.PricePerNight * boardPriceVariation, 2);

                        await _hotelSyncRepo.UpsertBoardPriceAsync(hprtId, boardTypeId.Value, adjustedBoardPrice);
                    }
                }

                count++;
            }

            await _providerRepo.SetSyncCompletedAsync(providerId);
            await InvalidateHotelCacheAsync();
            _logger.LogInformation("Sync completed for provider {ProviderId}: {Count} hotels processed, hotel cache invalidated", providerId, count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sync failed for provider {ProviderId}", providerId);
            await _providerRepo.SetSyncFailedAsync(providerId);
        }
    }

    private async Task InvalidateHotelCacheAsync()
    {
        var db = _redis.GetDatabase();
        var server = _redis.GetServers().First();
        var keys = new List<RedisKey>();

        await foreach (var key in server.KeysAsync(db.Database, "hotel:*"))
        {
            keys.Add(key);
            if (keys.Count >= 100)
            {
                await db.KeyDeleteAsync(keys.ToArray());
                keys.Clear();
            }
        }

        if (keys.Count > 0)
            await db.KeyDeleteAsync(keys.ToArray());
    }
}
