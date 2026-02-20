using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class AmenityCategoryConfiguration : IEntityTypeConfiguration<AmenityCategory>
{
    public void Configure(EntityTypeBuilder<AmenityCategory> builder)
    {
        builder.ToTable("amenity_category");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(50);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasIndex(e => e.Name).IsUnique();

        builder.HasMany(e => e.Amenities).WithOne(e => e.Category).HasForeignKey(e => e.CategoryId);
    }
}
