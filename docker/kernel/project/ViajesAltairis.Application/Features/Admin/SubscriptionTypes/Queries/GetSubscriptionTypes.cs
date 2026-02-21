using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.SubscriptionTypes.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.SubscriptionTypes.Queries;

public record GetSubscriptionTypesQuery : IRequest<IEnumerable<SubscriptionTypeDto>>;

public class GetSubscriptionTypesHandler : IRequestHandler<GetSubscriptionTypesQuery, IEnumerable<SubscriptionTypeDto>>
{
    private readonly IDbConnectionFactory _db;
    private readonly ITranslationService _translationService;
    private readonly ICurrentUserService _currentUserService;

    public GetSubscriptionTypesHandler(IDbConnectionFactory db, ITranslationService translationService, ICurrentUserService currentUserService)
    {
        _db = db;
        _translationService = translationService;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<SubscriptionTypeDto>> Handle(GetSubscriptionTypesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        var types = (await connection.QueryAsync<SubscriptionTypeDto>(
            @"SELECT id AS Id, name AS Name, price_per_month AS PricePerMonth, discount AS Discount,
                     currency_id AS CurrencyId, enabled AS Enabled, created_at AS CreatedAt, updated_at AS UpdatedAt
              FROM subscription_type ORDER BY name")).ToList();

        if (types.Count > 0)
        {
            var langId = _currentUserService.LanguageId;
            var ids = types.Select(t => t.Id).ToList();
            var names = await _translationService.ResolveAsync("subscription_type", ids, langId, "name", cancellationToken);
            foreach (var t in types)
                if (names.TryGetValue(t.Id, out var n)) t.Name = n;
        }

        return types;
    }
}
