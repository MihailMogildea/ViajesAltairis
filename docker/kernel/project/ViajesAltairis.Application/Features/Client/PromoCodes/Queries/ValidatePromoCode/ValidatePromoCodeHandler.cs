using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.PromoCodes.Queries.ValidatePromoCode;

public class ValidatePromoCodeHandler : IRequestHandler<ValidatePromoCodeQuery, ValidatePromoCodeResponse>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ICurrencyConverter _currencyConverter;
    private readonly ICurrentUserService _currentUserService;

    public ValidatePromoCodeHandler(
        IDbConnectionFactory connectionFactory,
        ICurrencyConverter currencyConverter,
        ICurrentUserService currentUserService)
    {
        _connectionFactory = connectionFactory;
        _currencyConverter = currencyConverter;
        _currentUserService = currentUserService;
    }

    public async Task<ValidatePromoCodeResponse> Handle(ValidatePromoCodeQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT
                promo_code_id AS Id,
                code AS Code,
                discount_percentage AS DiscountPercentage,
                discount_amount AS DiscountAmount,
                currency_code AS CurrencyCode,
                valid_to AS ValidTo
            FROM v_active_promo_code
            WHERE code = @Code
            """;

        var promo = await connection.QuerySingleOrDefaultAsync<PromoCodeRow>(sql, new { request.Code });

        if (promo == null)
        {
            return new ValidatePromoCodeResponse
            {
                IsValid = false,
                Message = "Promo code not found or expired."
            };
        }

        var discountAmount = promo.DiscountAmount;
        var currencyCode = promo.CurrencyCode;
        var targetCurrency = _currentUserService.CurrencyCode;

        // Convert amount-based discounts to user's preferred currency
        if (discountAmount.HasValue && !string.IsNullOrEmpty(currencyCode) && currencyCode != targetCurrency)
        {
            var currencyIds = await connection.QueryAsync<(long Id, string IsoCode)>(
                "SELECT id, iso_code FROM currency WHERE iso_code IN @Codes",
                new { Codes = new[] { currencyCode, targetCurrency } });
            var lookup = currencyIds.ToDictionary(c => c.IsoCode, c => c.Id);

            if (lookup.TryGetValue(currencyCode, out var sourceId)
                && lookup.TryGetValue(targetCurrency, out var targetId))
            {
                var (converted, _) = await _currencyConverter.ConvertAsync(discountAmount.Value, sourceId, targetId, cancellationToken);
                discountAmount = converted;
                currencyCode = targetCurrency;
            }
        }

        return new ValidatePromoCodeResponse
        {
            IsValid = true,
            DiscountPercentage = promo.DiscountPercentage,
            DiscountAmount = discountAmount,
            CurrencyCode = currencyCode ?? targetCurrency,
            ExpiresAt = promo.ValidTo?.ToDateTime(TimeOnly.MaxValue),
            Message = "Promo code is valid."
        };
    }

    private record PromoCodeRow(
        long Id,
        string Code,
        decimal? DiscountPercentage,
        decimal? DiscountAmount,
        string? CurrencyCode,
        DateOnly? ValidTo);
}
