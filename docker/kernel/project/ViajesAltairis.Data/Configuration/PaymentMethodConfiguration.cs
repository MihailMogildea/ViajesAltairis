using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("payment_method");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(50);
        builder.Property(e => e.MinDaysBeforeCheckin).HasColumnName("min_days_before_checkin").HasDefaultValue(0);
        builder.Property(e => e.Enabled).HasColumnName("enabled").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasIndex(e => e.Name).IsUnique();

        builder.HasMany(e => e.PaymentTransactions).WithOne(e => e.PaymentMethod).HasForeignKey(e => e.PaymentMethodId);
    }
}
