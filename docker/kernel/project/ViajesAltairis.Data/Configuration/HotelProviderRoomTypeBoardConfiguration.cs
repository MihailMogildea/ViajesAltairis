using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class HotelProviderRoomTypeBoardConfiguration : IEntityTypeConfiguration<HotelProviderRoomTypeBoard>
{
    public void Configure(EntityTypeBuilder<HotelProviderRoomTypeBoard> builder)
    {
        builder.ToTable("hotel_provider_room_type_board");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.HotelProviderRoomTypeId).HasColumnName("hotel_provider_room_type_id");
        builder.Property(e => e.BoardTypeId).HasColumnName("board_type_id");
        builder.Property(e => e.PricePerNight).HasColumnName("price_per_night").HasColumnType("decimal(10,2)").HasDefaultValue(0.00m);
        builder.Property(e => e.Enabled).HasColumnName("enabled").HasDefaultValue(true);

        builder.HasIndex(e => new { e.HotelProviderRoomTypeId, e.BoardTypeId }).IsUnique();

        builder.HasOne(e => e.HotelProviderRoomType).WithMany(e => e.HotelProviderRoomTypeBoards).HasForeignKey(e => e.HotelProviderRoomTypeId);
        builder.HasOne(e => e.BoardType).WithMany(e => e.HotelProviderRoomTypeBoards).HasForeignKey(e => e.BoardTypeId);
    }
}
