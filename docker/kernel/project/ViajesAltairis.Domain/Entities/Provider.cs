namespace ViajesAltairis.Domain.Entities;

public class Provider : BaseEntity
{
    public long TypeId { get; set; }
    public long CurrencyId { get; set; }
    public string Name { get; set; } = null!;
    public string? ApiUrl { get; set; }
    public string? ApiUsername { get; set; }
    public string? ApiPasswordEncrypted { get; set; }
    public decimal Margin { get; set; }
    public bool Enabled { get; set; }
    public string? SyncStatus { get; set; }
    public DateTime? LastSyncedAt { get; set; }

    public ProviderType Type { get; set; } = null!;
    public ICollection<HotelProvider> HotelProviders { get; set; } = [];
    public ICollection<User> Users { get; set; } = [];
}
