using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class RoomImageConfiguration : IEntityTypeConfiguration<RoomImage>
{
    public void Configure(EntityTypeBuilder<RoomImage> builder)
    {
        builder.ToTable("room_image");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.HotelProviderRoomTypeId).HasColumnName("hotel_provider_room_type_id");
        builder.Property(e => e.Url).HasColumnName("url").HasMaxLength(500);
        builder.Property(e => e.AltText).HasColumnName("alt_text").HasMaxLength(200);
        builder.Property(e => e.SortOrder).HasColumnName("sort_order").HasDefaultValue(0);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasOne(e => e.HotelProviderRoomType).WithMany(e => e.RoomImages).HasForeignKey(e => e.HotelProviderRoomTypeId);
    }
}
