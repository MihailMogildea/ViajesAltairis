using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class AdministrativeDivisionConfiguration : IEntityTypeConfiguration<AdministrativeDivision>
{
    public void Configure(EntityTypeBuilder<AdministrativeDivision> builder)
    {
        builder.ToTable("administrative_division");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.CountryId).HasColumnName("country_id").IsRequired();
        builder.Property(e => e.ParentId).HasColumnName("parent_id");
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(e => e.TypeId).HasColumnName("type_id").IsRequired();
        builder.Property(e => e.Level).HasColumnName("level").HasColumnType("tinyint").IsRequired();
        builder.Property(e => e.Enabled).HasColumnName("enabled").HasColumnType("tinyint(1)").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(e => e.Country)
            .WithMany(e => e.AdministrativeDivisions)
            .HasForeignKey(e => e.CountryId);

        builder.HasOne(e => e.Parent)
            .WithMany(e => e.Children)
            .HasForeignKey(e => e.ParentId);

        builder.HasOne(e => e.Type)
            .WithMany(e => e.AdministrativeDivisions)
            .HasForeignKey(e => e.TypeId);
    }
}
