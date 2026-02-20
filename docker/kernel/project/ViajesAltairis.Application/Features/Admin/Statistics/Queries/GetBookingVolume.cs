using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Statistics.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Statistics.Queries;

public record GetBookingVolumeQuery(DateTime? From, DateTime? To, string GroupBy = "month") : IRequest<IEnumerable<BookingVolumeDto>>;

public class GetBookingVolumeHandler : IRequestHandler<GetBookingVolumeQuery, IEnumerable<BookingVolumeDto>>
{
    private static readonly Dictionary<string, string> FormatMap = new()
    {
        ["day"] = "%Y-%m-%d",
        ["month"] = "%Y-%m"
    };

    private readonly IDbConnectionFactory _db;
    public GetBookingVolumeHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<BookingVolumeDto>> Handle(GetBookingVolumeQuery request, CancellationToken cancellationToken)
    {
        var format = FormatMap.GetValueOrDefault(request.GroupBy?.ToLowerInvariant() ?? "month", "%Y-%m");

        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<BookingVolumeDto>(
            $"""
            SELECT DATE_FORMAT(r.created_at, '{format}') AS Period, COUNT(*) AS BookingCount
            FROM reservation r
            WHERE (@From IS NULL OR r.created_at >= @From)
              AND (@To IS NULL OR r.created_at <= @To)
            GROUP BY Period
            ORDER BY Period
            """, new { request.From, request.To });
    }
}
