namespace ViajesAltairis.Application.Interfaces;

public interface IEcbRateParser
{
    Dictionary<string, decimal> ParseRates(string csv);
    decimal CalculateRateToEur(decimal ecbValue);
}
