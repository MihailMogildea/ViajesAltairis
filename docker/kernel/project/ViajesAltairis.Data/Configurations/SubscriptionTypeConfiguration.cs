using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class SubscriptionTypeConfiguration : IEntityTypeConfiguration<SubscriptionType>
{
    public void Configure(EntityTypeBuilder<SubscriptionType> builder)
    {
        builder.ToTable("subscription_type");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
        builder.Property(e => e.PricePerMonth).HasColumnName("price_per_month").HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(e => e.Discount).HasColumnName("discount").HasColumnType("decimal(5,2)").HasDefaultValue(0m);
        builder.Property(e => e.CurrencyId).HasColumnName("currency_id").IsRequired();
        builder.Property(e => e.Enabled).HasColumnName("enabled").HasColumnType("tinyint(1)").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();

        builder.HasOne(e => e.Currency)
            .WithMany()
            .HasForeignKey(e => e.CurrencyId);
    }
}
