namespace ViajesAltairis.Domain.Entities;

public class Translation : BaseEntity
{
    public string EntityType { get; set; } = null!;
    public long EntityId { get; set; }
    public string Field { get; set; } = null!;
    public long LanguageId { get; set; }
    public string Value { get; set; } = null!;

    public Language Language { get; set; } = null!;
}
