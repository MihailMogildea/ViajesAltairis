using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class HotelProviderRoomTypeAmenityConfiguration : IEntityTypeConfiguration<HotelProviderRoomTypeAmenity>
{
    public void Configure(EntityTypeBuilder<HotelProviderRoomTypeAmenity> builder)
    {
        builder.ToTable("hotel_provider_room_type_amenity");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(e => e.HotelProviderRoomTypeId).HasColumnName("hotel_provider_room_type_id").IsRequired();
        builder.Property(e => e.AmenityId).HasColumnName("amenity_id").IsRequired();

        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(e => new { e.HotelProviderRoomTypeId, e.AmenityId }).IsUnique();

        builder.HasOne(e => e.HotelProviderRoomType)
            .WithMany(e => e.HotelProviderRoomTypeAmenities)
            .HasForeignKey(e => e.HotelProviderRoomTypeId);

        builder.HasOne(e => e.Amenity)
            .WithMany(e => e.HotelProviderRoomTypeAmenities)
            .HasForeignKey(e => e.AmenityId);
    }
}
