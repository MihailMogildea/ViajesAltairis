using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Statistics.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Statistics.Queries;

public record GetBookingsByStatusQuery(DateTime? From, DateTime? To) : IRequest<IEnumerable<BookingsByStatusDto>>;

public class GetBookingsByStatusHandler : IRequestHandler<GetBookingsByStatusQuery, IEnumerable<BookingsByStatusDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetBookingsByStatusHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<BookingsByStatusDto>> Handle(GetBookingsByStatusQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<BookingsByStatusDto>(
            """
            SELECT rs.name AS StatusName, COUNT(r.id) AS BookingCount
            FROM reservation_status rs
            LEFT JOIN reservation r ON r.status_id = rs.id
              AND (@From IS NULL OR r.created_at >= @From)
              AND (@To IS NULL OR r.created_at <= @To)
            GROUP BY rs.id, rs.name
            ORDER BY rs.id
            """, new { request.From, request.To });
    }
}
