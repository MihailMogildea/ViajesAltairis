using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable("language");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.IsoCode).HasColumnName("iso_code").HasColumnType("char(2)").IsRequired();
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(e => e.IsoCode).IsUnique();
    }
}
