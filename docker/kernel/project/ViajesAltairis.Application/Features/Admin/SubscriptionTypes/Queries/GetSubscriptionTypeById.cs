using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.SubscriptionTypes.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.SubscriptionTypes.Queries;

public record GetSubscriptionTypeByIdQuery(long Id) : IRequest<SubscriptionTypeDto?>;

public class GetSubscriptionTypeByIdHandler : IRequestHandler<GetSubscriptionTypeByIdQuery, SubscriptionTypeDto?>
{
    private readonly IDbConnectionFactory _db;
    private readonly ITranslationService _translationService;
    private readonly ICurrentUserService _currentUserService;

    public GetSubscriptionTypeByIdHandler(IDbConnectionFactory db, ITranslationService translationService, ICurrentUserService currentUserService)
    {
        _db = db;
        _translationService = translationService;
        _currentUserService = currentUserService;
    }

    public async Task<SubscriptionTypeDto?> Handle(GetSubscriptionTypeByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        var type = await connection.QuerySingleOrDefaultAsync<SubscriptionTypeDto>(
            @"SELECT id AS Id, name AS Name, price_per_month AS PricePerMonth, discount AS Discount,
                     currency_id AS CurrencyId, enabled AS Enabled, created_at AS CreatedAt, updated_at AS UpdatedAt
              FROM subscription_type WHERE id = @Id",
            new { request.Id });

        if (type is not null)
        {
            var langId = _currentUserService.LanguageId;
            var names = await _translationService.ResolveAsync("subscription_type", new[] { type.Id }, langId, "name", cancellationToken);
            if (names.TryGetValue(type.Id, out var n)) type.Name = n;
        }

        return type;
    }
}
