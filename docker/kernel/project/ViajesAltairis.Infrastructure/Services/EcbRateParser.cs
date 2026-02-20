using System.Globalization;
using System.Text;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Infrastructure.Services;

public class EcbRateParser : IEcbRateParser
{
    public Dictionary<string, decimal> ParseRates(string csv)
    {
        var rates = new Dictionary<string, decimal>();

        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2)
            return rates;

        var headers = ParseCsvLine(lines[0]);
        var currencyCol = headers.IndexOf("CURRENCY");
        var valueCol = headers.IndexOf("OBS_VALUE");

        if (currencyCol < 0 || valueCol < 0)
            return rates;

        for (int i = 1; i < lines.Length; i++)
        {
            var cols = ParseCsvLine(lines[i]);
            if (cols.Count <= Math.Max(currencyCol, valueCol))
                continue;

            var isoCode = cols[currencyCol];
            if (decimal.TryParse(cols[valueCol], NumberStyles.Any, CultureInfo.InvariantCulture, out var ecbValue) && ecbValue > 0)
            {
                rates[isoCode] = ecbValue;
            }
        }

        return rates;
    }

    public decimal CalculateRateToEur(decimal ecbValue)
    {
        return Math.Round(1m / ecbValue, 6);
    }

    private static List<string> ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (inQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    current.Append(c);
                }
            }
            else if (c == '"')
            {
                inQuotes = true;
            }
            else if (c == ',')
            {
                fields.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        fields.Add(current.ToString());
        return fields;
    }
}
