using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("country");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.IsoCode).HasColumnName("iso_code").HasMaxLength(2).IsFixedLength();
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(100);
        builder.Property(e => e.CurrencyId).HasColumnName("currency_id");
        builder.Property(e => e.Enabled).HasColumnName("enabled").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasIndex(e => e.IsoCode).IsUnique();

        builder.HasOne(e => e.Currency).WithMany(e => e.Countries).HasForeignKey(e => e.CurrencyId);
        builder.HasMany(e => e.AdministrativeDivisions).WithOne(e => e.Country).HasForeignKey(e => e.CountryId);
    }
}
