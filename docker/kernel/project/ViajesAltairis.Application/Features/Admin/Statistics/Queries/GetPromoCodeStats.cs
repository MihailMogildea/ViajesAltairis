using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Statistics.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Statistics.Queries;

public record GetPromoCodeStatsQuery(DateTime? From, DateTime? To) : IRequest<IEnumerable<PromoCodeStatsDto>>;

public class GetPromoCodeStatsHandler : IRequestHandler<GetPromoCodeStatsQuery, IEnumerable<PromoCodeStatsDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetPromoCodeStatsHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<PromoCodeStatsDto>> Handle(GetPromoCodeStatsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<PromoCodeStatsDto>(
            """
            SELECT pc.id AS PromoCodeId, pc.code AS Code,
                   COUNT(r.id) AS UsageCount,
                   COALESCE(SUM(r.discount_amount), 0) AS TotalDiscount,
                   c.iso_code AS CurrencyCode
            FROM promo_code pc
            LEFT JOIN reservation r ON r.promo_code_id = pc.id
              AND (@From IS NULL OR r.created_at >= @From)
              AND (@To IS NULL OR r.created_at <= @To)
            LEFT JOIN currency c ON c.id = r.currency_id
            GROUP BY pc.id, pc.code, c.iso_code
            HAVING COUNT(r.id) > 0
            ORDER BY UsageCount DESC
            """, new { request.From, request.To });
    }
}
