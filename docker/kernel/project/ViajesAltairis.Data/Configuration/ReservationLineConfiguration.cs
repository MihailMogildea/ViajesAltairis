using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class ReservationLineConfiguration : IEntityTypeConfiguration<ReservationLine>
{
    public void Configure(EntityTypeBuilder<ReservationLine> builder)
    {
        builder.ToTable("reservation_line");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.ReservationId).HasColumnName("reservation_id");
        builder.Property(e => e.HotelProviderRoomTypeId).HasColumnName("hotel_provider_room_type_id");
        builder.Property(e => e.BoardTypeId).HasColumnName("board_type_id").HasDefaultValue(1L);
        builder.Property(e => e.CheckInDate).HasColumnName("check_in_date");
        builder.Property(e => e.CheckOutDate).HasColumnName("check_out_date");
        builder.Property(e => e.NumRooms).HasColumnName("num_rooms").HasDefaultValue(1);
        builder.Property(e => e.NumGuests).HasColumnName("num_guests");
        builder.Property(e => e.PricePerNight).HasColumnName("price_per_night").HasColumnType("decimal(10,2)");
        builder.Property(e => e.BoardPricePerNight).HasColumnName("board_price_per_night").HasColumnType("decimal(10,2)").HasDefaultValue(0.00m);
        builder.Property(e => e.NumNights).HasColumnName("num_nights");
        builder.Property(e => e.Subtotal).HasColumnName("subtotal").HasColumnType("decimal(10,2)");
        builder.Property(e => e.TaxAmount).HasColumnName("tax_amount").HasColumnType("decimal(10,2)");
        builder.Property(e => e.MarginAmount).HasColumnName("margin_amount").HasColumnType("decimal(10,2)");
        builder.Property(e => e.DiscountAmount).HasColumnName("discount_amount").HasColumnType("decimal(10,2)").HasDefaultValue(0.00m);
        builder.Property(e => e.TotalPrice).HasColumnName("total_price").HasColumnType("decimal(10,2)");
        builder.Property(e => e.CurrencyId).HasColumnName("currency_id");
        builder.Property(e => e.ExchangeRateId).HasColumnName("exchange_rate_id");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();

        builder.HasOne(e => e.Reservation).WithMany(e => e.ReservationLines).HasForeignKey(e => e.ReservationId);
        builder.HasOne(e => e.HotelProviderRoomType).WithMany(e => e.ReservationLines).HasForeignKey(e => e.HotelProviderRoomTypeId);
        builder.HasOne(e => e.BoardType).WithMany(e => e.ReservationLines).HasForeignKey(e => e.BoardTypeId);
        builder.HasOne(e => e.Currency).WithMany().HasForeignKey(e => e.CurrencyId);
        builder.HasOne(e => e.ExchangeRate).WithMany().HasForeignKey(e => e.ExchangeRateId);
        builder.HasMany(e => e.ReservationGuests).WithOne(e => e.ReservationLine).HasForeignKey(e => e.ReservationLineId);
    }
}
