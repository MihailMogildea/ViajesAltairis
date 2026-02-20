namespace ViajesAltairis.Domain.Entities;

public class Language : BaseEntity
{
    public string IsoCode { get; set; } = null!;
    public string Name { get; set; } = null!;

    public ICollection<Translation> Translations { get; set; } = [];
    public ICollection<WebTranslation> WebTranslations { get; set; } = [];
}
