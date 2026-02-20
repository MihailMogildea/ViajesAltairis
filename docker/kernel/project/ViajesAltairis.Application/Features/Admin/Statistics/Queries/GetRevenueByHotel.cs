using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Statistics.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Statistics.Queries;

public record GetRevenueByHotelQuery(DateTime? From, DateTime? To) : IRequest<IEnumerable<RevenueByHotelDto>>;

public class GetRevenueByHotelHandler : IRequestHandler<GetRevenueByHotelQuery, IEnumerable<RevenueByHotelDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetRevenueByHotelHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<RevenueByHotelDto>> Handle(GetRevenueByHotelQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<RevenueByHotelDto>(
            """
            SELECT h.id AS HotelId, h.name AS HotelName, c.iso_code AS CurrencyCode,
                   SUM(r.total_price) AS TotalRevenue, COUNT(DISTINCT r.id) AS ReservationCount
            FROM reservation r
            JOIN reservation_line rl ON rl.reservation_id = r.id
            JOIN hotel_provider_room_type hprt ON hprt.id = rl.hotel_provider_room_type_id
            JOIN hotel_provider hp ON hp.id = hprt.hotel_provider_id
            JOIN hotel h ON h.id = hp.hotel_id
            JOIN currency c ON c.id = r.currency_id
            WHERE r.status_id = 5
              AND (@From IS NULL OR r.created_at >= @From)
              AND (@To IS NULL OR r.created_at <= @To)
            GROUP BY h.id, h.name, c.iso_code
            ORDER BY TotalRevenue DESC
            """, new { request.From, request.To });
    }
}
