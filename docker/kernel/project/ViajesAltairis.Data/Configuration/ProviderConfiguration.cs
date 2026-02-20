using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class ProviderConfiguration : IEntityTypeConfiguration<Provider>
{
    public void Configure(EntityTypeBuilder<Provider> builder)
    {
        builder.ToTable("provider");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TypeId).HasColumnName("type_id");
        builder.Property(e => e.CurrencyId).HasColumnName("currency_id");
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(150);
        builder.Property(e => e.ApiUrl).HasColumnName("api_url").HasMaxLength(500);
        builder.Property(e => e.ApiUsername).HasColumnName("api_username").HasMaxLength(150);
        builder.Property(e => e.ApiPasswordEncrypted).HasColumnName("api_password_encrypted").HasMaxLength(500);
        builder.Property(e => e.Margin).HasColumnName("margin").HasColumnType("decimal(5,2)").HasDefaultValue(0.00m);
        builder.Property(e => e.Enabled).HasColumnName("enabled").HasDefaultValue(true);
        builder.Property(e => e.SyncStatus).HasColumnName("sync_status").HasMaxLength(20);
        builder.Property(e => e.LastSyncedAt).HasColumnName("last_synced_at");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasOne(e => e.Type).WithMany(e => e.Providers).HasForeignKey(e => e.TypeId);
        builder.HasMany(e => e.HotelProviders).WithOne(e => e.Provider).HasForeignKey(e => e.ProviderId);
        builder.HasMany(e => e.Users).WithOne(e => e.Provider).HasForeignKey(e => e.ProviderId);
    }
}
