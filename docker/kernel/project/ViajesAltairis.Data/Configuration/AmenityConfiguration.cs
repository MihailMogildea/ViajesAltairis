using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
{
    public void Configure(EntityTypeBuilder<Amenity> builder)
    {
        builder.ToTable("amenity");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CategoryId).HasColumnName("category_id");
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(100);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasOne(e => e.Category).WithMany(e => e.Amenities).HasForeignKey(e => e.CategoryId);
        builder.HasMany(e => e.HotelAmenities).WithOne(e => e.Amenity).HasForeignKey(e => e.AmenityId);
        builder.HasMany(e => e.HotelProviderRoomTypeAmenities).WithOne(e => e.Amenity).HasForeignKey(e => e.AmenityId);
    }
}
