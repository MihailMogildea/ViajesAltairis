using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class CancellationConfiguration : IEntityTypeConfiguration<Cancellation>
{
    public void Configure(EntityTypeBuilder<Cancellation> builder)
    {
        builder.ToTable("cancellation");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.ReservationId).HasColumnName("reservation_id");
        builder.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
        builder.Property(e => e.Reason).HasColumnName("reason").HasColumnType("text");
        builder.Property(e => e.PenaltyPercentage).HasColumnName("penalty_percentage").HasColumnType("decimal(5,2)").HasDefaultValue(0.00m);
        builder.Property(e => e.PenaltyAmount).HasColumnName("penalty_amount").HasColumnType("decimal(10,2)").HasDefaultValue(0.00m);
        builder.Property(e => e.RefundAmount).HasColumnName("refund_amount").HasColumnType("decimal(10,2)").HasDefaultValue(0.00m);
        builder.Property(e => e.CurrencyId).HasColumnName("currency_id");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasOne(e => e.Reservation).WithOne(e => e.Cancellation).HasForeignKey<Cancellation>(e => e.ReservationId);
        builder.HasOne(e => e.CancelledByUser).WithMany().HasForeignKey(e => e.CancelledByUserId);
        builder.HasOne(e => e.Currency).WithMany().HasForeignKey(e => e.CurrencyId);
    }
}
