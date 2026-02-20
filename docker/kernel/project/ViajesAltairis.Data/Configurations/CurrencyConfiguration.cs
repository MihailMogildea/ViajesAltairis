using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("currency");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.IsoCode).HasColumnName("iso_code").HasColumnType("char(3)").IsRequired();
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
        builder.Property(e => e.Symbol).HasColumnName("symbol").HasMaxLength(5).IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(e => e.IsoCode).IsUnique();
    }
}
