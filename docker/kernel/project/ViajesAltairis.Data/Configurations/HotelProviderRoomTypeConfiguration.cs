using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class HotelProviderRoomTypeConfiguration : IEntityTypeConfiguration<HotelProviderRoomType>
{
    public void Configure(EntityTypeBuilder<HotelProviderRoomType> builder)
    {
        builder.ToTable("hotel_provider_room_type");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.HotelProviderId).HasColumnName("hotel_provider_id").IsRequired();
        builder.Property(e => e.RoomTypeId).HasColumnName("room_type_id").IsRequired();
        builder.Property(e => e.Capacity).HasColumnName("capacity").HasColumnType("tinyint").IsRequired();
        builder.Property(e => e.Quantity).HasColumnName("quantity").IsRequired();
        builder.Property(e => e.PricePerNight).HasColumnName("price_per_night").HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(e => e.CurrencyId).HasColumnName("currency_id").IsRequired();
        builder.Property(e => e.ExchangeRateId).HasColumnName("exchange_rate_id").IsRequired();
        builder.Property(e => e.Enabled).HasColumnName("enabled").HasColumnType("tinyint(1)").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(e => new { e.HotelProviderId, e.RoomTypeId }).IsUnique();

        builder.HasOne(e => e.HotelProvider)
            .WithMany(e => e.HotelProviderRoomTypes)
            .HasForeignKey(e => e.HotelProviderId);

        builder.HasOne(e => e.RoomType)
            .WithMany(e => e.HotelProviderRoomTypes)
            .HasForeignKey(e => e.RoomTypeId);

        builder.HasOne(e => e.Currency)
            .WithMany()
            .HasForeignKey(e => e.CurrencyId);

        builder.HasOne(e => e.ExchangeRate)
            .WithMany()
            .HasForeignKey(e => e.ExchangeRateId);
    }
}
