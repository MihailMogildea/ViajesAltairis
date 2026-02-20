using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
{
    public void Configure(EntityTypeBuilder<ExchangeRate> builder)
    {
        builder.ToTable("exchange_rate");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CurrencyId).HasColumnName("currency_id");
        builder.Property(e => e.RateToEur).HasColumnName("rate_to_eur").HasColumnType("decimal(18,6)");
        builder.Property(e => e.ValidFrom).HasColumnName("valid_from");
        builder.Property(e => e.ValidTo).HasColumnName("valid_to");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasOne(e => e.Currency).WithMany(e => e.ExchangeRates).HasForeignKey(e => e.CurrencyId);
    }
}
