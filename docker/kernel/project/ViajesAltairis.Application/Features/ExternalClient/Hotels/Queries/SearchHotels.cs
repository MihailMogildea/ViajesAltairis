using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.ExternalClient.Hotels.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.ExternalClient.Hotels.Queries;

public record SearchHotelsQuery(
    string? City, string? Country, int? MinStars, int? MaxStars,
    int Page = 1, int PageSize = 20) : IRequest<SearchHotelsResponse>;

public record SearchHotelsResponse(List<HotelCardDto> Hotels, int TotalCount, int Page, int PageSize);

public class SearchHotelsHandler : IRequestHandler<SearchHotelsQuery, SearchHotelsResponse>
{
    private readonly IDbConnectionFactory _db;

    public SearchHotelsHandler(IDbConnectionFactory db) => _db = db;

    public async Task<SearchHotelsResponse> Handle(SearchHotelsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();

        var conditions = new List<string> { "enabled = TRUE" };
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            conditions.Add("city_name LIKE @City");
            parameters.Add("City", $"%{request.City}%");
        }

        if (!string.IsNullOrWhiteSpace(request.Country))
        {
            conditions.Add("country_name LIKE @Country");
            parameters.Add("Country", $"%{request.Country}%");
        }

        if (request.MinStars.HasValue)
        {
            conditions.Add("stars >= @MinStars");
            parameters.Add("MinStars", request.MinStars.Value);
        }

        if (request.MaxStars.HasValue)
        {
            conditions.Add("stars <= @MaxStars");
            parameters.Add("MaxStars", request.MaxStars.Value);
        }

        var where = string.Join(" AND ", conditions);
        var offset = (request.Page - 1) * request.PageSize;
        parameters.Add("Offset", offset);
        parameters.Add("PageSize", request.PageSize);

        var countSql = $"SELECT COUNT(*) FROM v_hotel_card WHERE {where}";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        var sql = $@"
            SELECT hotel_id AS HotelId, hotel_name AS HotelName, stars AS Stars,
                   city_id AS CityId, city_name AS CityName,
                   admin_division_name AS AdminDivisionName, country_name AS CountryName,
                   avg_rating AS AvgRating, review_count AS ReviewCount,
                   free_cancellation_hours AS FreeCancellationHours
            FROM v_hotel_card
            WHERE {where}
            ORDER BY hotel_name
            LIMIT @PageSize OFFSET @Offset";

        var hotels = (await connection.QueryAsync<HotelCardDto>(sql, parameters)).ToList();

        return new SearchHotelsResponse(hotels, totalCount, request.Page, request.PageSize);
    }
}
