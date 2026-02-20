using Dapper;
using MySqlConnector;

namespace ViajesAltairis.ProvidersApi.Repositories;

public class HotelSyncRepository : IHotelSyncRepository
{
    private readonly string _connectionString;

    public HotelSyncRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private MySqlConnection CreateConnection() => new(_connectionString);

    // --- City lookup ---

    public async Task<long?> GetCityIdByNameAsync(string cityName)
    {
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<long?>(
            "SELECT id FROM city WHERE name = @Name",
            new { Name = cityName });
    }

    // --- Hotel match/create ---

    public async Task<dynamic?> FindHotelAsync(string hotelName, string cityName)
    {
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync("""
            SELECT h.id, h.name, h.city_id, c.name AS city_name
            FROM hotel h
            JOIN city c ON c.id = h.city_id
            WHERE h.name = @HotelName AND c.name = @CityName
            """, new { HotelName = hotelName, CityName = cityName });
    }

    public async Task<long> InsertHotelAsync(long cityId, string name, int stars, string address, string? email, string? phone)
    {
        using var conn = CreateConnection();
        return await conn.ExecuteScalarAsync<long>("""
            INSERT INTO hotel (city_id, name, stars, address, email, phone)
            VALUES (@CityId, @Name, @Stars, @Address, @Email, @Phone);
            SELECT LAST_INSERT_ID();
            """, new { CityId = cityId, Name = name, Stars = stars, Address = address, Email = email, Phone = phone });
    }

    // --- Hotel-Provider link ---

    public async Task<long?> FindHotelProviderAsync(long hotelId, long providerId)
    {
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<long?>(
            "SELECT id FROM hotel_provider WHERE hotel_id = @HotelId AND provider_id = @ProviderId",
            new { HotelId = hotelId, ProviderId = providerId });
    }

    public async Task<long> InsertHotelProviderAsync(long hotelId, long providerId)
    {
        using var conn = CreateConnection();
        return await conn.ExecuteScalarAsync<long>("""
            INSERT INTO hotel_provider (hotel_id, provider_id) VALUES (@HotelId, @ProviderId);
            SELECT LAST_INSERT_ID();
            """, new { HotelId = hotelId, ProviderId = providerId });
    }

    // --- Room type lookup ---

    public async Task<long?> GetRoomTypeIdAsync(string name)
    {
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<long?>(
            "SELECT id FROM room_type WHERE name = @Name",
            new { Name = name });
    }

    // --- Currency / exchange rate lookup ---

    public async Task<long?> GetCurrencyIdByCodeAsync(string code)
    {
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<long?>(
            "SELECT id FROM currency WHERE iso_code = @Code",
            new { Code = code });
    }

    public async Task<long?> GetCurrentExchangeRateIdAsync(long currencyId)
    {
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<long?>(
            "SELECT id FROM exchange_rate WHERE currency_id = @Id AND NOW() BETWEEN valid_from AND valid_to LIMIT 1",
            new { Id = currencyId });
    }

    // --- Hotel-Provider-Room-Type upsert ---

    public async Task<long> UpsertHotelProviderRoomTypeAsync(long hotelProviderId, long roomTypeId, int capacity, int quantity, decimal pricePerNight, long currencyId, long exchangeRateId)
    {
        using var conn = CreateConnection();
        await conn.ExecuteAsync("""
            INSERT INTO hotel_provider_room_type (hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id)
            VALUES (@HpId, @RtId, @Capacity, @Quantity, @Price, @CurrencyId, @ExchangeRateId)
            ON DUPLICATE KEY UPDATE
                capacity = VALUES(capacity),
                quantity = VALUES(quantity),
                price_per_night = VALUES(price_per_night),
                currency_id = VALUES(currency_id),
                exchange_rate_id = VALUES(exchange_rate_id)
            """, new { HpId = hotelProviderId, RtId = roomTypeId, Capacity = capacity, Quantity = quantity, Price = pricePerNight, CurrencyId = currencyId, ExchangeRateId = exchangeRateId });

        return await conn.QueryFirstAsync<long>(
            "SELECT id FROM hotel_provider_room_type WHERE hotel_provider_id = @HpId AND room_type_id = @RtId",
            new { HpId = hotelProviderId, RtId = roomTypeId });
    }

    // --- Board type lookup ---

    public async Task<long?> GetBoardTypeIdAsync(string name)
    {
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<long?>(
            "SELECT id FROM board_type WHERE name = @Name",
            new { Name = name });
    }

    // --- Board price upsert ---

    public async Task UpsertBoardPriceAsync(long hprtId, long boardTypeId, decimal pricePerNight)
    {
        using var conn = CreateConnection();
        await conn.ExecuteAsync("""
            INSERT INTO hotel_provider_room_type_board (hotel_provider_room_type_id, board_type_id, price_per_night)
            VALUES (@HprtId, @BtId, @Price)
            ON DUPLICATE KEY UPDATE price_per_night = VALUES(price_per_night)
            """, new { HprtId = hprtId, BtId = boardTypeId, Price = pricePerNight });
    }

    // --- Search queries ---

    public async Task<IEnumerable<dynamic>> SearchHotelsAsync(string? city, int? stars)
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync("""
            SELECT h.id, h.name, h.stars, h.address, h.email, h.phone,
                   c.name AS city_name, h.enabled
            FROM hotel h
            JOIN city c ON c.id = h.city_id
            WHERE h.enabled = 1
              AND (@City IS NULL OR c.name = @City)
              AND (@Stars IS NULL OR h.stars = @Stars)
            ORDER BY c.name, h.stars DESC, h.name
            """, new { City = city, Stars = stars });
    }

    public async Task<dynamic?> GetHotelDetailAsync(long id)
    {
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync("""
            SELECT h.id, h.name, h.stars, h.address, h.email, h.phone,
                   h.check_in_time, h.check_out_time, h.latitude, h.longitude,
                   h.margin, c.name AS city_name
            FROM hotel h
            JOIN city c ON c.id = h.city_id
            WHERE h.id = @Id
            """, new { Id = id });
    }

    public async Task<IEnumerable<dynamic>> GetRoomTypesForHotelProviderAsync(long hotelId, long providerId)
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync("""
            SELECT hprt.id, rt.name AS room_type, hprt.capacity, hprt.quantity,
                   hprt.price_per_night, hprt.enabled
            FROM hotel_provider_room_type hprt
            JOIN hotel_provider hp ON hp.id = hprt.hotel_provider_id
            JOIN room_type rt ON rt.id = hprt.room_type_id
            WHERE hp.hotel_id = @HotelId AND hp.provider_id = @ProviderId
            ORDER BY hprt.price_per_night
            """, new { HotelId = hotelId, ProviderId = providerId });
    }

    public async Task<IEnumerable<dynamic>> GetBoardsForRoomTypeAsync(long hprtId)
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync("""
            SELECT bt.name AS board_type, b.price_per_night, b.enabled
            FROM hotel_provider_room_type_board b
            JOIN board_type bt ON bt.id = b.board_type_id
            WHERE b.hotel_provider_room_type_id = @HprtId
            ORDER BY b.price_per_night
            """, new { HprtId = hprtId });
    }
}
