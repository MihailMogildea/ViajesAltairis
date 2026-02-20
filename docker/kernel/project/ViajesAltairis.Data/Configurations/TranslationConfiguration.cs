using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class TranslationConfiguration : IEntityTypeConfiguration<Translation>
{
    public void Configure(EntityTypeBuilder<Translation> builder)
    {
        builder.ToTable("translation");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.EntityType).HasColumnName("entity_type").HasMaxLength(50).IsRequired();
        builder.Property(e => e.EntityId).HasColumnName("entity_id").IsRequired();
        builder.Property(e => e.Field).HasColumnName("field").HasMaxLength(50).IsRequired().HasDefaultValue("name");
        builder.Property(e => e.LanguageId).HasColumnName("language_id").IsRequired();
        builder.Property(e => e.Value).HasColumnName("value").HasColumnType("text").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(e => new { e.EntityType, e.EntityId, e.Field, e.LanguageId }).IsUnique();

        builder.HasOne(e => e.Language)
            .WithMany(e => e.Translations)
            .HasForeignKey(e => e.LanguageId);
    }
}
