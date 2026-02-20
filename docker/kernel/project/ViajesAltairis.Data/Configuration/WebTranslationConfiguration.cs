using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class WebTranslationConfiguration : IEntityTypeConfiguration<WebTranslation>
{
    public void Configure(EntityTypeBuilder<WebTranslation> builder)
    {
        builder.ToTable("web_translation");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TranslationKey).HasColumnName("translation_key").HasMaxLength(150);
        builder.Property(e => e.LanguageId).HasColumnName("language_id");
        builder.Property(e => e.Value).HasColumnName("value").HasColumnType("text");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasIndex(e => new { e.TranslationKey, e.LanguageId }).IsUnique();

        builder.HasOne(e => e.Language).WithMany(e => e.WebTranslations).HasForeignKey(e => e.LanguageId);
    }
}
