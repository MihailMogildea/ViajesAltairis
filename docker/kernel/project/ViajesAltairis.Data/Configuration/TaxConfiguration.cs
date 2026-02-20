using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class TaxConfiguration : IEntityTypeConfiguration<Tax>
{
    public void Configure(EntityTypeBuilder<Tax> builder)
    {
        builder.ToTable("tax");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TaxTypeId).HasColumnName("tax_type_id");
        builder.Property(e => e.CountryId).HasColumnName("country_id");
        builder.Property(e => e.AdministrativeDivisionId).HasColumnName("administrative_division_id");
        builder.Property(e => e.CityId).HasColumnName("city_id");
        builder.Property(e => e.Rate).HasColumnName("rate").HasColumnType("decimal(10,4)");
        builder.Property(e => e.IsPercentage).HasColumnName("is_percentage").HasDefaultValue(true);
        builder.Property(e => e.Enabled).HasColumnName("enabled").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasOne(e => e.TaxType).WithMany(e => e.Taxes).HasForeignKey(e => e.TaxTypeId);
        builder.HasOne(e => e.Country).WithMany().HasForeignKey(e => e.CountryId);
        builder.HasOne(e => e.AdministrativeDivision).WithMany().HasForeignKey(e => e.AdministrativeDivisionId);
        builder.HasOne(e => e.City).WithMany().HasForeignKey(e => e.CityId);
    }
}
