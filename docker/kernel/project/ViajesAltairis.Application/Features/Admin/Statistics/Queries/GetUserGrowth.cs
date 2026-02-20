using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Statistics.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Statistics.Queries;

public record GetUserGrowthQuery(DateTime? From, DateTime? To, string GroupBy = "month") : IRequest<IEnumerable<UserGrowthDto>>;

public class GetUserGrowthHandler : IRequestHandler<GetUserGrowthQuery, IEnumerable<UserGrowthDto>>
{
    private static readonly Dictionary<string, string> FormatMap = new()
    {
        ["day"] = "%Y-%m-%d",
        ["month"] = "%Y-%m"
    };

    private readonly IDbConnectionFactory _db;
    public GetUserGrowthHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<UserGrowthDto>> Handle(GetUserGrowthQuery request, CancellationToken cancellationToken)
    {
        var format = FormatMap.GetValueOrDefault(request.GroupBy?.ToLowerInvariant() ?? "month", "%Y-%m");

        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<UserGrowthDto>(
            $"""
            SELECT DATE_FORMAT(u.created_at, '{format}') AS Period, COUNT(*) AS NewUsers
            FROM user u
            WHERE u.user_type_id = 5
              AND (@From IS NULL OR u.created_at >= @From)
              AND (@To IS NULL OR u.created_at <= @To)
            GROUP BY Period
            ORDER BY Period
            """, new { request.From, request.To });
    }
}
