using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Reference.Queries.GetPaymentMethods;

public class GetPaymentMethodsHandler : IRequestHandler<GetPaymentMethodsQuery, GetPaymentMethodsResponse>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ITranslationService _translationService;
    private readonly ICurrentUserService _currentUserService;

    public GetPaymentMethodsHandler(
        IDbConnectionFactory connectionFactory,
        ITranslationService translationService,
        ICurrentUserService currentUserService)
    {
        _connectionFactory = connectionFactory;
        _translationService = translationService;
        _currentUserService = currentUserService;
    }

    public async Task<GetPaymentMethodsResponse> Handle(GetPaymentMethodsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT id AS Id, name AS Code, name AS Name, min_days_before_checkin AS MinDaysBeforeCheckin
            FROM payment_method
            WHERE enabled = 1
            ORDER BY id
            """;

        var methods = (await connection.QueryAsync<PaymentMethodDto>(sql)).ToList();

        // Always resolve translations — DB name stores keys like "bank_transfer", not display names
        var langId = _currentUserService.LanguageId;
        var ids = methods.Select(m => m.Id).ToList();
        var translations = await _translationService.ResolveAsync("payment_method", ids, langId, "name", cancellationToken);
        foreach (var method in methods)
        {
            if (translations.TryGetValue(method.Id, out var translated))
                method.Name = translated;
        }

        return new GetPaymentMethodsResponse { PaymentMethods = methods };
    }
}
