using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
{
    public void Configure(EntityTypeBuilder<Amenity> builder)
    {
        builder.ToTable("amenity");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(e => e.CategoryId).HasColumnName("category_id").IsRequired();
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();

        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(e => e.Category)
            .WithMany(e => e.Amenities)
            .HasForeignKey(e => e.CategoryId);
    }
}
