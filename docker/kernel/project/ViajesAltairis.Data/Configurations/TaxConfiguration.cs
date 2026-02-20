using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class TaxConfiguration : IEntityTypeConfiguration<Tax>
{
    public void Configure(EntityTypeBuilder<Tax> builder)
    {
        builder.ToTable("tax");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(e => e.TaxTypeId).HasColumnName("tax_type_id").IsRequired();
        builder.Property(e => e.CountryId).HasColumnName("country_id");
        builder.Property(e => e.AdministrativeDivisionId).HasColumnName("administrative_division_id");
        builder.Property(e => e.CityId).HasColumnName("city_id");
        builder.Property(e => e.Rate).HasColumnName("rate").HasColumnType("decimal(10,4)").IsRequired();
        builder.Property(e => e.IsPercentage).HasColumnName("is_percentage").HasColumnType("tinyint(1)").HasDefaultValue(true);
        builder.Property(e => e.Enabled).HasColumnName("enabled").HasColumnType("tinyint(1)").HasDefaultValue(true);

        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(e => e.TaxType)
            .WithMany(e => e.Taxes)
            .HasForeignKey(e => e.TaxTypeId);

        builder.HasOne(e => e.Country)
            .WithMany()
            .HasForeignKey(e => e.CountryId);

        builder.HasOne(e => e.AdministrativeDivision)
            .WithMany()
            .HasForeignKey(e => e.AdministrativeDivisionId);

        builder.HasOne(e => e.City)
            .WithMany()
            .HasForeignKey(e => e.CityId);
    }
}
