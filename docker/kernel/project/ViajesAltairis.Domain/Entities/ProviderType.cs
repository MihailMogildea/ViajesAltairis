namespace ViajesAltairis.Domain.Entities;

public class ProviderType : BaseEntity
{
    public string Name { get; set; } = null!;

    public ICollection<Provider> Providers { get; set; } = [];
}
