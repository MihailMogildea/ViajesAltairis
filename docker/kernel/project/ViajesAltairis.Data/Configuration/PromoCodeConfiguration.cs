using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
{
    public void Configure(EntityTypeBuilder<PromoCode> builder)
    {
        builder.ToTable("promo_code");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Code).HasColumnName("code").HasMaxLength(50);
        builder.Property(e => e.DiscountPercentage).HasColumnName("discount_percentage").HasColumnType("decimal(5,2)");
        builder.Property(e => e.DiscountAmount).HasColumnName("discount_amount").HasColumnType("decimal(10,2)");
        builder.Property(e => e.CurrencyId).HasColumnName("currency_id");
        builder.Property(e => e.ValidFrom).HasColumnName("valid_from");
        builder.Property(e => e.ValidTo).HasColumnName("valid_to");
        builder.Property(e => e.MaxUses).HasColumnName("max_uses");
        builder.Property(e => e.CurrentUses).HasColumnName("current_uses").HasDefaultValue(0);
        builder.Property(e => e.Enabled).HasColumnName("enabled").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();

        builder.HasIndex(e => e.Code).IsUnique();

        builder.HasOne(e => e.Currency).WithMany().HasForeignKey(e => e.CurrencyId);
        builder.HasMany(e => e.Reservations).WithOne(e => e.PromoCode).HasForeignKey(e => e.PromoCodeId);
    }
}
