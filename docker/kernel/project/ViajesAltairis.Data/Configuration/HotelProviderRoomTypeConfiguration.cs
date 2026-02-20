using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class HotelProviderRoomTypeConfiguration : IEntityTypeConfiguration<HotelProviderRoomType>
{
    public void Configure(EntityTypeBuilder<HotelProviderRoomType> builder)
    {
        builder.ToTable("hotel_provider_room_type");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.HotelProviderId).HasColumnName("hotel_provider_id");
        builder.Property(e => e.RoomTypeId).HasColumnName("room_type_id");
        builder.Property(e => e.Capacity).HasColumnName("capacity");
        builder.Property(e => e.Quantity).HasColumnName("quantity");
        builder.Property(e => e.PricePerNight).HasColumnName("price_per_night").HasColumnType("decimal(10,2)");
        builder.Property(e => e.CurrencyId).HasColumnName("currency_id");
        builder.Property(e => e.ExchangeRateId).HasColumnName("exchange_rate_id");
        builder.Property(e => e.Enabled).HasColumnName("enabled").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasIndex(e => new { e.HotelProviderId, e.RoomTypeId }).IsUnique();

        builder.HasOne(e => e.HotelProvider).WithMany(e => e.HotelProviderRoomTypes).HasForeignKey(e => e.HotelProviderId);
        builder.HasOne(e => e.RoomType).WithMany(e => e.HotelProviderRoomTypes).HasForeignKey(e => e.RoomTypeId);
        builder.HasOne(e => e.Currency).WithMany().HasForeignKey(e => e.CurrencyId);
        builder.HasOne(e => e.ExchangeRate).WithMany().HasForeignKey(e => e.ExchangeRateId);
        builder.HasMany(e => e.HotelProviderRoomTypeAmenities).WithOne(e => e.HotelProviderRoomType).HasForeignKey(e => e.HotelProviderRoomTypeId);
        builder.HasMany(e => e.RoomImages).WithOne(e => e.HotelProviderRoomType).HasForeignKey(e => e.HotelProviderRoomTypeId);
        builder.HasMany(e => e.HotelProviderRoomTypeBoards).WithOne(e => e.HotelProviderRoomType).HasForeignKey(e => e.HotelProviderRoomTypeId);
        builder.HasMany(e => e.ReservationLines).WithOne(e => e.HotelProviderRoomType).HasForeignKey(e => e.HotelProviderRoomTypeId);
    }
}
