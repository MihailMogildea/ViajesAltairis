using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class PaymentTransactionFeeConfiguration : IEntityTypeConfiguration<PaymentTransactionFee>
{
    public void Configure(EntityTypeBuilder<PaymentTransactionFee> builder)
    {
        builder.ToTable("payment_transaction_fee");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.PaymentTransactionId).HasColumnName("payment_transaction_id");
        builder.Property(e => e.FeeType).HasColumnName("fee_type").HasMaxLength(50);
        builder.Property(e => e.FeeAmount).HasColumnName("fee_amount").HasColumnType("decimal(10,2)");
        builder.Property(e => e.FeePercentage).HasColumnName("fee_percentage").HasColumnType("decimal(5,4)");
        builder.Property(e => e.FixedFeeAmount).HasColumnName("fixed_fee_amount").HasColumnType("decimal(10,2)");
        builder.Property(e => e.CurrencyId).HasColumnName("currency_id");
        builder.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasOne(e => e.PaymentTransaction).WithMany(e => e.PaymentTransactionFees).HasForeignKey(e => e.PaymentTransactionId);
        builder.HasOne(e => e.Currency).WithMany().HasForeignKey(e => e.CurrencyId);
    }
}
