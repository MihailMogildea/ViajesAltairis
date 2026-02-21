using System.Security.Cryptography;
using System.Text;
using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Hotels.Queries.SearchHotels;

public class SearchHotelsHandler : IRequestHandler<SearchHotelsQuery, SearchHotelsResponse>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ITranslationService _translationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICurrencyConverter _currencyConverter;
    private readonly ICacheService _cacheService;

    public SearchHotelsHandler(
        IDbConnectionFactory connectionFactory,
        ITranslationService translationService,
        ICurrentUserService currentUserService,
        ICurrencyConverter currencyConverter,
        ICacheService cacheService)
    {
        _connectionFactory = connectionFactory;
        _translationService = translationService;
        _currentUserService = currentUserService;
        _currencyConverter = currencyConverter;
        _cacheService = cacheService;
    }

    public async Task<SearchHotelsResponse> Handle(SearchHotelsQuery request, CancellationToken cancellationToken)
    {
        var langId = _currentUserService.LanguageId;
        var currency = _currentUserService.CurrencyCode;
        var amenityKey = request.AmenityIds is { Count: > 0 } ? string.Join(",", request.AmenityIds.Order()) : "";
        var paramsString = $"{request.CityId}:{request.CountryId}:{request.CheckIn}:{request.CheckOut}:{request.Guests}:{request.Stars}:{amenityKey}:{request.Page}:{request.PageSize}:{langId}:{currency}";
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(paramsString)));
        var cacheKey = $"hotel:search:{hash}";

        var cached = await _cacheService.GetAsync<SearchHotelsResponse>(cacheKey, cancellationToken);
        if (cached != null)
            return cached;
        using var connection = _connectionFactory.CreateConnection();

        var whereClauses = new List<string> { "h.enabled = TRUE" };
        var parameters = new DynamicParameters();

        if (request.CityId.HasValue)
        {
            whereClauses.Add("h.city_id = @CityId");
            parameters.Add("CityId", request.CityId.Value);
        }

        if (request.CountryId.HasValue)
        {
            whereClauses.Add("h.country_id = @CountryId");
            parameters.Add("CountryId", request.CountryId.Value);
        }

        if (request.Stars.HasValue)
        {
            whereClauses.Add("h.stars = @Stars");
            parameters.Add("Stars", request.Stars.Value);
        }

        if (request.AmenityIds is { Count: > 0 })
        {
            whereClauses.Add($"""
                h.hotel_id IN (
                    SELECT ha.hotel_id FROM hotel_amenity ha
                    WHERE ha.amenity_id IN @AmenityIds
                    GROUP BY ha.hotel_id
                    HAVING COUNT(DISTINCT ha.amenity_id) = @AmenityCount
                )
                """);
            parameters.Add("AmenityIds", request.AmenityIds);
            parameters.Add("AmenityCount", request.AmenityIds.Count);
        }

        var where = string.Join(" AND ", whereClauses);
        var offset = (request.Page - 1) * request.PageSize;

        parameters.Add("Offset", offset);
        parameters.Add("PageSize", request.PageSize);

        var countSql = $"SELECT COUNT(DISTINCT h.hotel_id) FROM v_hotel_card h WHERE {where}";

        var dataSql = $"""
            SELECT
                h.hotel_id AS Id,
                h.hotel_name AS Name,
                h.stars AS Stars,
                h.city_id AS CityId,
                h.city_name AS City,
                h.city_image_url AS CityImageUrl,
                h.country_id AS CountryId,
                h.country_name AS Country,
                h.avg_rating AS AvgRating,
                h.review_count AS ReviewCount,
                (SELECT MIN(rc.price_per_night * (1 + (rc.provider_margin + rc.hotel_margin) / 100) * COALESCE(er.rate_to_eur, 1))
                 FROM v_hotel_room_catalog rc
                 LEFT JOIN exchange_rate er ON er.currency_id = (
                     SELECT cur.id FROM currency cur WHERE cur.iso_code = rc.currency_code
                 ) AND er.valid_from <= NOW() AND er.valid_to > NOW()
                 WHERE rc.hotel_id = h.hotel_id AND rc.enabled = TRUE) AS PriceFrom,
                (SELECT hi.url
                 FROM hotel_image hi
                 WHERE hi.hotel_id = h.hotel_id
                 ORDER BY hi.sort_order
                 LIMIT 1) AS MainImageUrl
            FROM v_hotel_card h
            WHERE {where}
            ORDER BY h.hotel_name
            LIMIT @PageSize OFFSET @Offset
            """;

        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
        var hotels = (await connection.QueryAsync<HotelSummaryDto>(dataSql, parameters)).ToList();

        if (hotels.Count > 0)
        {
            // Hotel summaries (always from translation table)
            var hotelIds = hotels.Select(h => h.Id).ToList();
            var summaries = await _translationService.ResolveAsync("hotel", hotelIds, langId, "summary", cancellationToken);
            foreach (var h in hotels)
                if (summaries.TryGetValue(h.Id, out var s)) h.Summary = s;

            // City and country names (translate only for non-English)
            if (langId != 1)
            {
                var cityIds = hotels.Select(h => h.CityId).Distinct().ToList();
                var countryIds = hotels.Select(h => h.CountryId).Distinct().ToList();

                var cityNames = await _translationService.ResolveAsync("city", cityIds, langId, "name", cancellationToken);
                var countryNames = await _translationService.ResolveAsync("country", countryIds, langId, "name", cancellationToken);

                foreach (var h in hotels)
                {
                    if (cityNames.TryGetValue(h.CityId, out var cn)) h.City = cn;
                    if (countryNames.TryGetValue(h.CountryId, out var con)) h.Country = con;
                }
            }
        }

        // Currency conversion: PriceFrom is in EUR from the SQL query
        var targetCurrency = _currentUserService.CurrencyCode;
        if (hotels.Count > 0 && hotels.Any(h => h.PriceFrom.HasValue))
        {
            var targetCurrencyId = await connection.ExecuteScalarAsync<long>(
                "SELECT id FROM currency WHERE iso_code = @Code", new { Code = targetCurrency });

            if (targetCurrencyId > 0 && targetCurrencyId != 1) // 1 = EUR
            {
                var (factor, _) = await _currencyConverter.ConvertAsync(1m, 1, targetCurrencyId, cancellationToken);
                foreach (var h in hotels)
                    if (h.PriceFrom.HasValue)
                        h.PriceFrom = Math.Round(h.PriceFrom.Value * factor, 2);
            }
        }

        var response = new SearchHotelsResponse
        {
            Hotels = hotels,
            TotalCount = totalCount,
            CurrencyCode = targetCurrency
        };

        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10), cancellationToken);
        return response;
    }
}
