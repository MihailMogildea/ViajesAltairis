using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Subscriptions.Queries.GetSubscriptionPlans;

public class GetSubscriptionPlansHandler : IRequestHandler<GetSubscriptionPlansQuery, GetSubscriptionPlansResponse>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ITranslationService _translationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICurrencyConverter _currencyConverter;
    private readonly ICacheService _cacheService;

    public GetSubscriptionPlansHandler(
        IDbConnectionFactory connectionFactory,
        ITranslationService translationService,
        ICurrentUserService currentUserService,
        ICurrencyConverter currencyConverter,
        ICacheService cacheService)
    {
        _connectionFactory = connectionFactory;
        _translationService = translationService;
        _currentUserService = currentUserService;
        _currencyConverter = currencyConverter;
        _cacheService = cacheService;
    }

    public async Task<GetSubscriptionPlansResponse> Handle(GetSubscriptionPlansQuery request, CancellationToken cancellationToken)
    {
        var langId = _currentUserService.LanguageId;
        var currency = _currentUserService.CurrencyCode;
        var cacheKey = $"ref:subscriptions:{langId}:{currency}";

        var cached = await _cacheService.GetAsync<GetSubscriptionPlansResponse>(cacheKey, cancellationToken);
        if (cached != null)
            return cached;
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT
                st.id AS Id,
                st.name AS Name,
                st.price_per_month AS Price,
                st.discount AS Discount,
                st.currency_id AS CurrencyId,
                c.iso_code AS CurrencyCode
            FROM subscription_type st
            JOIN currency c ON c.id = st.currency_id
            WHERE st.enabled = TRUE
            ORDER BY st.price_per_month
            """;

        var plans = (await connection.QueryAsync<SubscriptionPlanDto>(sql)).ToList();

        if (langId != 1 && plans.Count > 0)
        {
            var ids = plans.Select(p => p.Id).ToList();
            var names = await _translationService.ResolveAsync("subscription_type", ids, langId, "name", cancellationToken);
            foreach (var p in plans)
                if (names.TryGetValue(p.Id, out var n)) p.Name = n;
        }

        // Currency conversion
        var targetCurrency = _currentUserService.CurrencyCode;
        if (plans.Count > 0)
        {
            var targetCurrencyId = await connection.ExecuteScalarAsync<long>(
                "SELECT id FROM currency WHERE iso_code = @Code", new { Code = targetCurrency });

            if (targetCurrencyId > 0)
            {
                foreach (var p in plans)
                {
                    if (p.CurrencyId != targetCurrencyId)
                    {
                        var (converted, _) = await _currencyConverter.ConvertAsync(p.Price, p.CurrencyId, targetCurrencyId, cancellationToken);
                        p.Price = converted;
                    }
                    p.CurrencyCode = targetCurrency;
                }
            }
        }

        var response = new GetSubscriptionPlansResponse { Plans = plans };
        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromHours(1), cancellationToken);
        return response;
    }
}
