using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Hotels.Queries.GetHotelReviews;

public class GetHotelReviewsHandler : IRequestHandler<GetHotelReviewsQuery, GetHotelReviewsResponse>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetHotelReviewsHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<GetHotelReviewsResponse> Handle(GetHotelReviewsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        var offset = (request.Page - 1) * request.PageSize;

        const string statsSql = """
            SELECT
                COALESCE(AVG(rating), 0) AS AvgRating,
                COUNT(*) AS TotalCount
            FROM v_hotel_review_detail
            WHERE hotel_id = @HotelId AND visible = TRUE
            """;

        var stats = await connection.QuerySingleAsync<StatsRow>(statsSql, new { request.HotelId });

        const string reviewsSql = """
            SELECT
                review_id AS Id,
                CONCAT(reviewer_first_name, ' ', LEFT(reviewer_last_name, 1), '.') AS UserName,
                rating AS Rating,
                title AS Title,
                comment AS Comment,
                review_created_at AS CreatedAt,
                response_comment AS ResponseComment,
                response_created_at AS ResponseDate
            FROM v_hotel_review_detail
            WHERE hotel_id = @HotelId AND visible = TRUE
            ORDER BY review_created_at DESC
            LIMIT @PageSize OFFSET @Offset
            """;

        var reviews = (await connection.QueryAsync<ReviewDto>(reviewsSql, new
        {
            request.HotelId,
            PageSize = request.PageSize,
            Offset = offset
        })).ToList();

        return new GetHotelReviewsResponse
        {
            Reviews = reviews,
            TotalCount = (int)stats.TotalCount,
            AverageRating = stats.AvgRating
        };
    }

    private class StatsRow
    {
        public decimal AvgRating { get; set; }
        public long TotalCount { get; set; }
    }
}
