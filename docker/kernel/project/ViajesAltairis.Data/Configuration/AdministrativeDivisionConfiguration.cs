using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class AdministrativeDivisionConfiguration : IEntityTypeConfiguration<AdministrativeDivision>
{
    public void Configure(EntityTypeBuilder<AdministrativeDivision> builder)
    {
        builder.ToTable("administrative_division");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CountryId).HasColumnName("country_id");
        builder.Property(e => e.ParentId).HasColumnName("parent_id");
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(150);
        builder.Property(e => e.TypeId).HasColumnName("type_id");
        builder.Property(e => e.Level).HasColumnName("level");
        builder.Property(e => e.Enabled).HasColumnName("enabled").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasOne(e => e.Country).WithMany(e => e.AdministrativeDivisions).HasForeignKey(e => e.CountryId);
        builder.HasOne(e => e.Parent).WithMany(e => e.Children).HasForeignKey(e => e.ParentId);
        builder.HasOne(e => e.Type).WithMany(e => e.AdministrativeDivisions).HasForeignKey(e => e.TypeId);
        builder.HasMany(e => e.Cities).WithOne(e => e.AdministrativeDivision).HasForeignKey(e => e.AdministrativeDivisionId);
        builder.HasMany(e => e.SeasonalMargins).WithOne(e => e.AdministrativeDivision).HasForeignKey(e => e.AdministrativeDivisionId);
    }
}
