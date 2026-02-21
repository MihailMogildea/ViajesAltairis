using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable("payment_transaction");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.ReservationId).HasColumnName("reservation_id").IsRequired();
        builder.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id").IsRequired();
        builder.Property(e => e.TransactionReference).HasColumnName("transaction_reference").HasMaxLength(255).IsRequired();
        builder.Property(e => e.Amount).HasColumnName("amount").HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(e => e.CurrencyId).HasColumnName("currency_id").IsRequired();
        builder.Property(e => e.ExchangeRateId).HasColumnName("exchange_rate_id").IsRequired();
        builder.Property(e => e.StatusId).HasColumnName("status_id").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();

        builder.HasOne(e => e.Reservation)
            .WithMany(e => e.PaymentTransactions)
            .HasForeignKey(e => e.ReservationId);

        builder.HasOne(e => e.PaymentMethod)
            .WithMany(e => e.PaymentTransactions)
            .HasForeignKey(e => e.PaymentMethodId);

        builder.HasOne(e => e.Currency)
            .WithMany()
            .HasForeignKey(e => e.CurrencyId);

        builder.HasOne(e => e.ExchangeRate)
            .WithMany()
            .HasForeignKey(e => e.ExchangeRateId);
    }
}
