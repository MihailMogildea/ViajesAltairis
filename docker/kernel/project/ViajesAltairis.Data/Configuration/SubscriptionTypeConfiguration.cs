using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class SubscriptionTypeConfiguration : IEntityTypeConfiguration<SubscriptionType>
{
    public void Configure(EntityTypeBuilder<SubscriptionType> builder)
    {
        builder.ToTable("subscription_type");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(50);
        builder.Property(e => e.PricePerMonth).HasColumnName("price_per_month").HasColumnType("decimal(10,2)");
        builder.Property(e => e.Discount).HasColumnName("discount").HasColumnType("decimal(5,2)").HasDefaultValue(0.00m);
        builder.Property(e => e.CurrencyId).HasColumnName("currency_id");
        builder.Property(e => e.Enabled).HasColumnName("enabled").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();

        builder.HasOne(e => e.Currency).WithMany().HasForeignKey(e => e.CurrencyId);
        builder.HasMany(e => e.UserSubscriptions).WithOne(e => e.SubscriptionType).HasForeignKey(e => e.SubscriptionTypeId);
    }
}
