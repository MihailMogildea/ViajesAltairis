using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("reservation");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.ReservationCode).HasColumnName("reservation_code").HasMaxLength(20).IsRequired();
        builder.Property(e => e.StatusId).HasColumnName("status_id").IsRequired();
        builder.Property(e => e.BookedByUserId).HasColumnName("booked_by_user_id").IsRequired();
        builder.Property(e => e.OwnerUserId).HasColumnName("owner_user_id");
        builder.Property(e => e.OwnerFirstName).HasColumnName("owner_first_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.OwnerLastName).HasColumnName("owner_last_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.OwnerEmail).HasColumnName("owner_email").HasMaxLength(200);
        builder.Property(e => e.OwnerPhone).HasColumnName("owner_phone").HasMaxLength(50);
        builder.Property(e => e.OwnerTaxId).HasColumnName("owner_tax_id").HasMaxLength(50);
        builder.Property(e => e.OwnerAddress).HasColumnName("owner_address").HasMaxLength(300);
        builder.Property(e => e.OwnerCity).HasColumnName("owner_city").HasMaxLength(100);
        builder.Property(e => e.OwnerPostalCode).HasColumnName("owner_postal_code").HasMaxLength(20);
        builder.Property(e => e.OwnerCountry).HasColumnName("owner_country").HasMaxLength(100);
        builder.Property(e => e.Subtotal).HasColumnName("subtotal").HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(e => e.TaxAmount).HasColumnName("tax_amount").HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(e => e.MarginAmount).HasColumnName("margin_amount").HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(e => e.DiscountAmount).HasColumnName("discount_amount").HasColumnType("decimal(10,2)").HasDefaultValue(0m);
        builder.Property(e => e.TotalPrice).HasColumnName("total_price").HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(e => e.CurrencyId).HasColumnName("currency_id").IsRequired();
        builder.Property(e => e.ExchangeRateId).HasColumnName("exchange_rate_id").IsRequired();
        builder.Property(e => e.PromoCodeId).HasColumnName("promo_code_id");
        builder.Property(e => e.Notes).HasColumnName("notes").HasColumnType("text");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();

        builder.HasIndex(e => e.ReservationCode).IsUnique();

        builder.HasOne(e => e.Status)
            .WithMany(e => e.Reservations)
            .HasForeignKey(e => e.StatusId);

        builder.HasOne(e => e.BookedByUser)
            .WithMany()
            .HasForeignKey(e => e.BookedByUserId);

        builder.HasOne(e => e.OwnerUser)
            .WithMany()
            .HasForeignKey(e => e.OwnerUserId);

        builder.HasOne(e => e.Currency)
            .WithMany()
            .HasForeignKey(e => e.CurrencyId);

        builder.HasOne(e => e.ExchangeRate)
            .WithMany()
            .HasForeignKey(e => e.ExchangeRateId);

        builder.HasOne(e => e.PromoCode)
            .WithMany(e => e.Reservations)
            .HasForeignKey(e => e.PromoCodeId);
    }
}
