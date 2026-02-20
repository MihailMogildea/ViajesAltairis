using ViajesAltairis.Infrastructure.Services;

namespace ViajesAltairis.Scheduled.Api.Tests.Services;

public class EcbRateParserTests
{
    private readonly EcbRateParser _parser = new();

    [Fact]
    public void ParseRates_ValidCsv_ReturnsCorrectRates()
    {
        var csv = "KEY,FREQ,CURRENCY,CURRENCY_DENOM,EXR_TYPE,EXR_SUFFIX,TIME_PERIOD,OBS_VALUE\n" +
                  "EXR.D.GBP.EUR.SP00.A,D,GBP,EUR,SP00,A,2024-01-15,0.8724";

        var rates = _parser.ParseRates(csv);

        rates.Should().ContainKey("GBP");
        rates["GBP"].Should().Be(0.8724m);
    }

    [Fact]
    public void ParseRates_QuotedFields_HandlesCorrectly()
    {
        var csv = "KEY,FREQ,CURRENCY,CURRENCY_DENOM,EXR_TYPE,EXR_SUFFIX,TIME_PERIOD,OBS_VALUE\n" +
                  "\"EXR.D.USD.EUR.SP00.A\",D,USD,EUR,SP00,A,2024-01-15,1.0876";

        var rates = _parser.ParseRates(csv);

        rates.Should().ContainKey("USD");
        rates["USD"].Should().Be(1.0876m);
    }

    [Fact]
    public void ParseRates_EmptyCsv_ReturnsEmptyDictionary()
    {
        var rates = _parser.ParseRates("");
        rates.Should().BeEmpty();
    }

    [Fact]
    public void ParseRates_HeaderOnly_ReturnsEmptyDictionary()
    {
        var csv = "KEY,FREQ,CURRENCY,CURRENCY_DENOM,EXR_TYPE,EXR_SUFFIX,TIME_PERIOD,OBS_VALUE";
        var rates = _parser.ParseRates(csv);
        rates.Should().BeEmpty();
    }

    [Fact]
    public void ParseRates_MissingCurrencyColumn_ReturnsEmptyDictionary()
    {
        var csv = "KEY,FREQ,SOME_COL,OBS_VALUE\ndata,D,X,1.23";
        var rates = _parser.ParseRates(csv);
        rates.Should().BeEmpty();
    }

    [Fact]
    public void ParseRates_MissingObsValueColumn_ReturnsEmptyDictionary()
    {
        var csv = "KEY,FREQ,CURRENCY,SOME_COL\ndata,D,USD,1.23";
        var rates = _parser.ParseRates(csv);
        rates.Should().BeEmpty();
    }

    [Fact]
    public void ParseRates_InvalidObsValue_SkipsRow()
    {
        var csv = "KEY,FREQ,CURRENCY,CURRENCY_DENOM,EXR_TYPE,EXR_SUFFIX,TIME_PERIOD,OBS_VALUE\n" +
                  "EXR.D.GBP.EUR.SP00.A,D,GBP,EUR,SP00,A,2024-01-15,invalid\n" +
                  "EXR.D.USD.EUR.SP00.A,D,USD,EUR,SP00,A,2024-01-15,1.0876";

        var rates = _parser.ParseRates(csv);

        rates.Should().NotContainKey("GBP");
        rates.Should().ContainKey("USD");
    }

    [Fact]
    public void ParseRates_NegativeObsValue_SkipsRow()
    {
        var csv = "KEY,FREQ,CURRENCY,CURRENCY_DENOM,EXR_TYPE,EXR_SUFFIX,TIME_PERIOD,OBS_VALUE\n" +
                  "EXR.D.GBP.EUR.SP00.A,D,GBP,EUR,SP00,A,2024-01-15,-0.5";

        var rates = _parser.ParseRates(csv);
        rates.Should().BeEmpty();
    }

    [Fact]
    public void ParseRates_MultipleCurrencies_ReturnsAll()
    {
        var csv = "KEY,FREQ,CURRENCY,CURRENCY_DENOM,EXR_TYPE,EXR_SUFFIX,TIME_PERIOD,OBS_VALUE\n" +
                  "EXR.D.GBP.EUR.SP00.A,D,GBP,EUR,SP00,A,2024-01-15,0.8724\n" +
                  "EXR.D.USD.EUR.SP00.A,D,USD,EUR,SP00,A,2024-01-15,1.0876\n" +
                  "EXR.D.JPY.EUR.SP00.A,D,JPY,EUR,SP00,A,2024-01-15,160.53";

        var rates = _parser.ParseRates(csv);

        rates.Should().HaveCount(3);
        rates.Should().ContainKeys("GBP", "USD", "JPY");
    }

    [Fact]
    public void CalculateRateToEur_GbpRate_ReturnsCorrectInverse()
    {
        var result = _parser.CalculateRateToEur(0.8724m);
        result.Should().Be(1.146263m);
    }

    [Fact]
    public void CalculateRateToEur_OneToOne_ReturnsOne()
    {
        var result = _parser.CalculateRateToEur(1.0m);
        result.Should().Be(1.0m);
    }

    [Fact]
    public void CalculateRateToEur_LargeValue_ReturnsCorrectInverse()
    {
        var result = _parser.CalculateRateToEur(160.53m);
        result.Should().Be(Math.Round(1m / 160.53m, 6));
    }
}
