using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Hotels.Queries.GetHotelDetail;

public class GetHotelDetailHandler : IRequestHandler<GetHotelDetailQuery, GetHotelDetailResponse>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ITranslationService _translationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICacheService _cacheService;

    public GetHotelDetailHandler(
        IDbConnectionFactory connectionFactory,
        ITranslationService translationService,
        ICurrentUserService currentUserService,
        ICacheService cacheService)
    {
        _connectionFactory = connectionFactory;
        _translationService = translationService;
        _currentUserService = currentUserService;
        _cacheService = cacheService;
    }

    public async Task<GetHotelDetailResponse> Handle(GetHotelDetailQuery request, CancellationToken cancellationToken)
    {
        var langId = _currentUserService.LanguageId;
        var cacheKey = $"hotel:detail:{request.HotelId}:{langId}";

        var cached = await _cacheService.GetAsync<GetHotelDetailResponse>(cacheKey, cancellationToken);
        if (cached != null)
            return cached;
        using var connection = _connectionFactory.CreateConnection();

        const string hotelSql = """
            SELECT
                hotel_id AS Id,
                hotel_name AS Name,
                stars AS Stars,
                address AS Address,
                city_id AS CityId,
                city_name AS City,
                country_id AS CountryId,
                country_name AS Country,
                latitude AS Latitude,
                longitude AS Longitude,
                phone AS Phone,
                email AS Email,
                CAST(check_in_time AS CHAR) AS CheckInTime,
                CAST(check_out_time AS CHAR) AS CheckOutTime,
                avg_rating AS AvgRating,
                review_count AS ReviewCount,
                free_cancellation_hours AS FreeCancellationHours
            FROM v_hotel_detail
            WHERE hotel_id = @HotelId AND enabled = TRUE
            """;

        var hotel = await connection.QuerySingleOrDefaultAsync<GetHotelDetailResponse>(hotelSql, new { request.HotelId });
        if (hotel == null)
            throw new KeyNotFoundException($"Hotel {request.HotelId} not found.");

        const string imagesSql = "SELECT url FROM hotel_image WHERE hotel_id = @HotelId ORDER BY sort_order, id";
        hotel.Images = (await connection.QueryAsync<string>(imagesSql, new { request.HotelId })).ToList();

        const string amenitiesSql = "SELECT amenity_id AS Id, amenity_name AS Name FROM v_hotel_amenity_list WHERE hotel_id = @HotelId";
        hotel.Amenities = (await connection.QueryAsync<AmenityDto>(amenitiesSql, new { request.HotelId })).ToList();

        // Translations

        // Hotel summary (always from translation table)
        var summaries = await _translationService.ResolveAsync("hotel", [hotel.Id], langId, "summary", cancellationToken);
        if (summaries.TryGetValue(hotel.Id, out var summary)) hotel.Summary = summary;

        if (langId != 1)
        {
            // City and country names
            var cityNames = await _translationService.ResolveAsync("city", [hotel.CityId], langId, "name", cancellationToken);
            var countryNames = await _translationService.ResolveAsync("country", [hotel.CountryId], langId, "name", cancellationToken);

            if (cityNames.TryGetValue(hotel.CityId, out var cn)) hotel.City = cn;
            if (countryNames.TryGetValue(hotel.CountryId, out var con)) hotel.Country = con;

            // Amenity names
            if (hotel.Amenities.Count > 0)
            {
                var amenityIds = hotel.Amenities.Select(a => a.Id).ToList();
                var amenityNames = await _translationService.ResolveAsync("amenity", amenityIds, langId, "name", cancellationToken);
                foreach (var a in hotel.Amenities)
                    if (amenityNames.TryGetValue(a.Id, out var an)) a.Name = an;
            }
        }

        await _cacheService.SetAsync(cacheKey, hotel, TimeSpan.FromMinutes(30), cancellationToken);
        return hotel;
    }
}
