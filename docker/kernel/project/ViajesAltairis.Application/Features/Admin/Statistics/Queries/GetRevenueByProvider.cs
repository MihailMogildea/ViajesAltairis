using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Statistics.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Statistics.Queries;

public record GetRevenueByProviderQuery(DateTime? From, DateTime? To) : IRequest<IEnumerable<RevenueByProviderDto>>;

public class GetRevenueByProviderHandler : IRequestHandler<GetRevenueByProviderQuery, IEnumerable<RevenueByProviderDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetRevenueByProviderHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<RevenueByProviderDto>> Handle(GetRevenueByProviderQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<RevenueByProviderDto>(
            """
            SELECT p.id AS ProviderId, p.name AS ProviderName, c.iso_code AS CurrencyCode,
                   SUM(r.total_price) AS TotalRevenue, COUNT(DISTINCT r.id) AS ReservationCount
            FROM reservation r
            JOIN reservation_line rl ON rl.reservation_id = r.id
            JOIN hotel_provider_room_type hprt ON hprt.id = rl.hotel_provider_room_type_id
            JOIN hotel_provider hp ON hp.id = hprt.hotel_provider_id
            JOIN provider p ON p.id = hp.provider_id
            JOIN currency c ON c.id = r.currency_id
            WHERE r.status_id = 5
              AND (@From IS NULL OR r.created_at >= @From)
              AND (@To IS NULL OR r.created_at <= @To)
            GROUP BY p.id, p.name, c.iso_code
            ORDER BY TotalRevenue DESC
            """, new { request.From, request.To });
    }
}
