using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
{
    public void Configure(EntityTypeBuilder<ExchangeRate> builder)
    {
        builder.ToTable("exchange_rate");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.CurrencyId).HasColumnName("currency_id").IsRequired();
        builder.Property(e => e.RateToEur).HasColumnName("rate_to_eur").HasColumnType("decimal(18,6)").IsRequired();
        builder.Property(e => e.ValidFrom).HasColumnName("valid_from").HasColumnType("datetime").IsRequired();
        builder.Property(e => e.ValidTo).HasColumnName("valid_to").HasColumnType("datetime").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(e => e.Currency)
            .WithMany(e => e.ExchangeRates)
            .HasForeignKey(e => e.CurrencyId);
    }
}
