using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class HotelAmenityConfiguration : IEntityTypeConfiguration<HotelAmenity>
{
    public void Configure(EntityTypeBuilder<HotelAmenity> builder)
    {
        builder.ToTable("hotel_amenity");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.HotelId).HasColumnName("hotel_id");
        builder.Property(e => e.AmenityId).HasColumnName("amenity_id");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasIndex(e => new { e.HotelId, e.AmenityId }).IsUnique();

        builder.HasOne(e => e.Hotel).WithMany(e => e.HotelAmenities).HasForeignKey(e => e.HotelId);
        builder.HasOne(e => e.Amenity).WithMany(e => e.HotelAmenities).HasForeignKey(e => e.AmenityId);
    }
}
