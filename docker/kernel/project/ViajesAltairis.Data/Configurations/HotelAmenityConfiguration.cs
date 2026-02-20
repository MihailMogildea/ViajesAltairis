using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class HotelAmenityConfiguration : IEntityTypeConfiguration<HotelAmenity>
{
    public void Configure(EntityTypeBuilder<HotelAmenity> builder)
    {
        builder.ToTable("hotel_amenity");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(e => e.HotelId).HasColumnName("hotel_id").IsRequired();
        builder.Property(e => e.AmenityId).HasColumnName("amenity_id").IsRequired();

        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(e => new { e.HotelId, e.AmenityId }).IsUnique();

        builder.HasOne(e => e.Hotel)
            .WithMany(e => e.HotelAmenities)
            .HasForeignKey(e => e.HotelId);

        builder.HasOne(e => e.Amenity)
            .WithMany(e => e.HotelAmenities)
            .HasForeignKey(e => e.AmenityId);
    }
}
