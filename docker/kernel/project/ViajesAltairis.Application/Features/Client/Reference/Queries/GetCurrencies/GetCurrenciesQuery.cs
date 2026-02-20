using MediatR;

namespace ViajesAltairis.Application.Features.Client.Reference.Queries.GetCurrencies;

public class GetCurrenciesQuery : IRequest<GetCurrenciesResponse>
{
}

public class GetCurrenciesResponse
{
    public List<CurrencyDto> Currencies { get; set; } = new();
}

public class CurrencyDto
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public decimal? ExchangeRateToEur { get; set; }
}
