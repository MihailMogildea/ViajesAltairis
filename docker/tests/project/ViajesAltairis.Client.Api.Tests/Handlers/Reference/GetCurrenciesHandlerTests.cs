using FluentAssertions;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Reference.Queries.GetCurrencies;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Client.Api.Tests.Fixtures;

namespace ViajesAltairis.Client.Api.Tests.Handlers.Reference;

public class GetCurrenciesHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCurrenciesWithExchangeRates()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "currency", "id INTEGER PRIMARY KEY, iso_code TEXT, name TEXT, symbol TEXT");
        TestDbHelper.CreateTable(conn, "v_exchange_rate_current", "currency_id INTEGER, rate_to_eur REAL");
        TestDbHelper.Execute(conn, "INSERT INTO currency VALUES (1, 'EUR', 'Euro', '€'), (2, 'USD', 'US Dollar', '$')");
        TestDbHelper.Execute(conn, "INSERT INTO v_exchange_rate_current VALUES (2, 0.92)");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);

        var handler = new GetCurrenciesHandler(factory);
        var result = await handler.Handle(new GetCurrenciesQuery(), CancellationToken.None);

        result.Currencies.Should().HaveCount(2);
        result.Currencies.First(c => c.Code == "USD").ExchangeRateToEur.Should().Be(0.92m);
        result.Currencies.First(c => c.Code == "EUR").ExchangeRateToEur.Should().BeNull();
    }
}
