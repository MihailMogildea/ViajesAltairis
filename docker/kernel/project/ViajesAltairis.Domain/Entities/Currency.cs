namespace ViajesAltairis.Domain.Entities;

public class Currency : BaseEntity
{
    public string IsoCode { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Symbol { get; set; } = null!;

    public ICollection<ExchangeRate> ExchangeRates { get; set; } = [];
    public ICollection<Country> Countries { get; set; } = [];
}
