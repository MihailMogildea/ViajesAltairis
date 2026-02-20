using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class WebTranslationConfiguration : IEntityTypeConfiguration<WebTranslation>
{
    public void Configure(EntityTypeBuilder<WebTranslation> builder)
    {
        builder.ToTable("web_translation");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.TranslationKey).HasColumnName("translation_key").HasMaxLength(150).IsRequired();
        builder.Property(e => e.LanguageId).HasColumnName("language_id").IsRequired();
        builder.Property(e => e.Value).HasColumnName("value").HasColumnType("text").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(e => new { e.TranslationKey, e.LanguageId }).IsUnique();

        builder.HasOne(e => e.Language)
            .WithMany(e => e.WebTranslations)
            .HasForeignKey(e => e.LanguageId);
    }
}
