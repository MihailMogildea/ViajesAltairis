using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class PaymentTransactionFeeConfiguration : IEntityTypeConfiguration<PaymentTransactionFee>
{
    public void Configure(EntityTypeBuilder<PaymentTransactionFee> builder)
    {
        builder.ToTable("payment_transaction_fee");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.PaymentTransactionId).HasColumnName("payment_transaction_id").IsRequired();
        builder.Property(e => e.FeeType).HasColumnName("fee_type").HasMaxLength(50).IsRequired();
        builder.Property(e => e.FeeAmount).HasColumnName("fee_amount").HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(e => e.FeePercentage).HasColumnName("fee_percentage").HasColumnType("decimal(5,4)");
        builder.Property(e => e.FixedFeeAmount).HasColumnName("fixed_fee_amount").HasColumnType("decimal(10,2)");
        builder.Property(e => e.CurrencyId).HasColumnName("currency_id").IsRequired();
        builder.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(e => e.PaymentTransaction)
            .WithMany(e => e.PaymentTransactionFees)
            .HasForeignKey(e => e.PaymentTransactionId);

        builder.HasOne(e => e.Currency)
            .WithMany()
            .HasForeignKey(e => e.CurrencyId);
    }
}
