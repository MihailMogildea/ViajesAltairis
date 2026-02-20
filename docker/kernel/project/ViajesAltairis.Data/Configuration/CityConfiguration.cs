using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("city");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.AdministrativeDivisionId).HasColumnName("administrative_division_id");
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(150);
        builder.Property(e => e.Enabled).HasColumnName("enabled").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasOne(e => e.AdministrativeDivision).WithMany(e => e.Cities).HasForeignKey(e => e.AdministrativeDivisionId);
        builder.HasMany(e => e.Hotels).WithOne(e => e.City).HasForeignKey(e => e.CityId);
    }
}
