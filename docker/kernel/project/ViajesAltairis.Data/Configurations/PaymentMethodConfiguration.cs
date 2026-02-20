using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("payment_method");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
        builder.Property(e => e.MinDaysBeforeCheckin).HasColumnName("min_days_before_checkin").HasDefaultValue(0);
        builder.Property(e => e.Enabled).HasColumnName("enabled").HasColumnType("tinyint(1)").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(e => e.Name).IsUnique();
    }
}
