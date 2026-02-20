using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Statistics.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Statistics.Queries;

public record GetCancellationStatsQuery(DateTime? From, DateTime? To) : IRequest<CancellationStatsDto>;

public class GetCancellationStatsHandler : IRequestHandler<GetCancellationStatsQuery, CancellationStatsDto>
{
    private readonly IDbConnectionFactory _db;
    public GetCancellationStatsHandler(IDbConnectionFactory db) => _db = db;

    public async Task<CancellationStatsDto> Handle(GetCancellationStatsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleAsync<CancellationStatsDto>(
            """
            SELECT
                (SELECT COUNT(*) FROM cancellation
                 WHERE (@From IS NULL OR created_at >= @From) AND (@To IS NULL OR created_at <= @To)) AS CancellationCount,
                (SELECT COUNT(*) FROM reservation
                 WHERE (@From IS NULL OR created_at >= @From) AND (@To IS NULL OR created_at <= @To)) AS TotalReservations,
                CASE WHEN (SELECT COUNT(*) FROM reservation
                           WHERE (@From IS NULL OR created_at >= @From) AND (@To IS NULL OR created_at <= @To)) > 0
                     THEN ROUND((SELECT COUNT(*) FROM cancellation
                                 WHERE (@From IS NULL OR created_at >= @From) AND (@To IS NULL OR created_at <= @To)) * 100.0
                          / (SELECT COUNT(*) FROM reservation
                             WHERE (@From IS NULL OR created_at >= @From) AND (@To IS NULL OR created_at <= @To)), 2)
                     ELSE 0 END AS CancellationRate,
                COALESCE((SELECT SUM(penalty_amount) FROM cancellation
                          WHERE (@From IS NULL OR created_at >= @From) AND (@To IS NULL OR created_at <= @To)), 0) AS TotalPenalty,
                COALESCE((SELECT SUM(refund_amount) FROM cancellation
                          WHERE (@From IS NULL OR created_at >= @From) AND (@To IS NULL OR created_at <= @To)), 0) AS TotalRefund
            """, new { request.From, request.To });
    }
}
