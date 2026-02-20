using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class AmenityCategoryConfiguration : IEntityTypeConfiguration<AmenityCategory>
{
    public void Configure(EntityTypeBuilder<AmenityCategory> builder)
    {
        builder.ToTable("amenity_category");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
        builder.HasIndex(e => e.Name).IsUnique();

        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
