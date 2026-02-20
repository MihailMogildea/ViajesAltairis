using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Statistics.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Statistics.Queries;

public record GetRevenueByPeriodQuery(DateTime? From, DateTime? To, string GroupBy = "month") : IRequest<IEnumerable<RevenuePeriodDto>>;

public class GetRevenueByPeriodHandler : IRequestHandler<GetRevenueByPeriodQuery, IEnumerable<RevenuePeriodDto>>
{
    private static readonly Dictionary<string, string> FormatMap = new()
    {
        ["day"] = "%Y-%m-%d",
        ["month"] = "%Y-%m"
    };

    private readonly IDbConnectionFactory _db;
    public GetRevenueByPeriodHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<RevenuePeriodDto>> Handle(GetRevenueByPeriodQuery request, CancellationToken cancellationToken)
    {
        var format = FormatMap.GetValueOrDefault(request.GroupBy?.ToLowerInvariant() ?? "month", "%Y-%m");

        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<RevenuePeriodDto>(
            $"""
            SELECT DATE_FORMAT(r.created_at, '{format}') AS Period, c.iso_code AS CurrencyCode,
                   SUM(r.total_price) AS TotalRevenue, COUNT(*) AS ReservationCount
            FROM reservation r
            JOIN currency c ON c.id = r.currency_id
            WHERE r.status_id = 5
              AND (@From IS NULL OR r.created_at >= @From)
              AND (@To IS NULL OR r.created_at <= @To)
            GROUP BY Period, c.iso_code
            ORDER BY Period
            """, new { request.From, request.To });
    }
}
