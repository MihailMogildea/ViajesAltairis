namespace ViajesAltairis.Domain.Entities;

public class WebTranslation : BaseEntity
{
    public string TranslationKey { get; set; } = null!;
    public long LanguageId { get; set; }
    public string Value { get; set; } = null!;

    public Language Language { get; set; } = null!;
}
