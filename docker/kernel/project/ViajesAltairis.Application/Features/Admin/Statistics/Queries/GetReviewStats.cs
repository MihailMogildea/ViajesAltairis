using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Statistics.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Statistics.Queries;

public record GetReviewStatsQuery(DateTime? From, DateTime? To) : IRequest<ReviewStatsDto>;

public class GetReviewStatsHandler : IRequestHandler<GetReviewStatsQuery, ReviewStatsDto>
{
    private readonly IDbConnectionFactory _db;
    public GetReviewStatsHandler(IDbConnectionFactory db) => _db = db;

    public async Task<ReviewStatsDto> Handle(GetReviewStatsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleAsync<ReviewStatsDto>(
            """
            SELECT COALESCE(AVG(rating), 0) AS AverageRating,
                   COUNT(*) AS TotalReviews,
                   SUM(CASE WHEN rating = 1 THEN 1 ELSE 0 END) AS Rating1,
                   SUM(CASE WHEN rating = 2 THEN 1 ELSE 0 END) AS Rating2,
                   SUM(CASE WHEN rating = 3 THEN 1 ELSE 0 END) AS Rating3,
                   SUM(CASE WHEN rating = 4 THEN 1 ELSE 0 END) AS Rating4,
                   SUM(CASE WHEN rating = 5 THEN 1 ELSE 0 END) AS Rating5
            FROM review
            WHERE (@From IS NULL OR created_at >= @From)
              AND (@To IS NULL OR created_at <= @To)
            """, new { request.From, request.To });
    }
}
