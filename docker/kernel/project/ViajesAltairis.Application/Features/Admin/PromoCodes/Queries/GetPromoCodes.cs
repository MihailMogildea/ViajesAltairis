using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.PromoCodes.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.PromoCodes.Queries;

public record GetPromoCodesQuery : IRequest<IEnumerable<PromoCodeDto>>;

public class GetPromoCodesHandler : IRequestHandler<GetPromoCodesQuery, IEnumerable<PromoCodeDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetPromoCodesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<PromoCodeDto>> Handle(GetPromoCodesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<PromoCodeDto>(
            "SELECT id AS Id, code AS Code, discount_percentage AS DiscountPercentage, discount_amount AS DiscountAmount, currency_id AS CurrencyId, valid_from AS ValidFrom, valid_to AS ValidTo, max_uses AS MaxUses, current_uses AS CurrentUses, enabled AS Enabled, created_at AS CreatedAt, updated_at AS UpdatedAt FROM promo_code ORDER BY code");
    }
}
