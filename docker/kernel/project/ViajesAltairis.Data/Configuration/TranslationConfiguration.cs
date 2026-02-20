using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class TranslationConfiguration : IEntityTypeConfiguration<Translation>
{
    public void Configure(EntityTypeBuilder<Translation> builder)
    {
        builder.ToTable("translation");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.EntityType).HasColumnName("entity_type").HasMaxLength(50);
        builder.Property(e => e.EntityId).HasColumnName("entity_id");
        builder.Property(e => e.Field).HasColumnName("field").HasMaxLength(50).HasDefaultValue("name");
        builder.Property(e => e.LanguageId).HasColumnName("language_id");
        builder.Property(e => e.Value).HasColumnName("value").HasColumnType("text");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasIndex(e => new { e.EntityType, e.EntityId, e.Field, e.LanguageId }).IsUnique();

        builder.HasOne(e => e.Language).WithMany(e => e.Translations).HasForeignKey(e => e.LanguageId);
    }
}
