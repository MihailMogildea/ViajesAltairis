using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Statistics.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Statistics.Queries;

public record GetOccupancyQuery(DateTime? From, DateTime? To) : IRequest<IEnumerable<OccupancyDto>>;

public class GetOccupancyHandler : IRequestHandler<GetOccupancyQuery, IEnumerable<OccupancyDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetOccupancyHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<OccupancyDto>> Handle(GetOccupancyQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<OccupancyDto>(
            """
            SELECT h.id AS HotelId, h.name AS HotelName,
                   COALESCE(SUM(rl.num_rooms * rl.num_nights), 0) AS BookedRoomNights,
                   COALESCE(SUM(hprt.quantity), 0) * DATEDIFF(COALESCE(@To, CURDATE()), COALESCE(@From, DATE_SUB(CURDATE(), INTERVAL 30 DAY))) AS TotalRoomNights,
                   CASE WHEN COALESCE(SUM(hprt.quantity), 0) * DATEDIFF(COALESCE(@To, CURDATE()), COALESCE(@From, DATE_SUB(CURDATE(), INTERVAL 30 DAY))) > 0
                        THEN ROUND(COALESCE(SUM(rl.num_rooms * rl.num_nights), 0) * 100.0
                             / (COALESCE(SUM(hprt.quantity), 0) * DATEDIFF(COALESCE(@To, CURDATE()), COALESCE(@From, DATE_SUB(CURDATE(), INTERVAL 30 DAY)))), 2)
                        ELSE 0 END AS OccupancyRate
            FROM hotel h
            JOIN hotel_provider hp ON hp.hotel_id = h.id
            JOIN hotel_provider_room_type hprt ON hprt.hotel_provider_id = hp.id AND hprt.enabled = 1
            LEFT JOIN reservation_line rl ON rl.hotel_provider_room_type_id = hprt.id
                AND rl.check_in_date < COALESCE(@To, CURDATE())
                AND rl.check_out_date > COALESCE(@From, DATE_SUB(CURDATE(), INTERVAL 30 DAY))
            LEFT JOIN reservation r ON r.id = rl.reservation_id AND r.status_id IN (2, 3, 4, 5)
            WHERE h.enabled = 1
            GROUP BY h.id, h.name
            ORDER BY OccupancyRate DESC
            """, new { request.From, request.To });
    }
}
