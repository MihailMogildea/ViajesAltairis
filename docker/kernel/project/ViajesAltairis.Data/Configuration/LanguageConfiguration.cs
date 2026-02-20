using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable("language");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.IsoCode).HasColumnName("iso_code").HasMaxLength(2).IsFixedLength();
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(50);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasIndex(e => e.IsoCode).IsUnique();

        builder.HasMany(e => e.Translations).WithOne(e => e.Language).HasForeignKey(e => e.LanguageId);
        builder.HasMany(e => e.WebTranslations).WithOne(e => e.Language).HasForeignKey(e => e.LanguageId);
    }
}
