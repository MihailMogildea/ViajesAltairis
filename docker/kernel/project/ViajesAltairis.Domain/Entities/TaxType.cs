namespace ViajesAltairis.Domain.Entities;

public class TaxType : BaseEntity
{
    public string Name { get; set; } = null!;

    public ICollection<Tax> Taxes { get; set; } = [];
}
