using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class HotelProviderConfiguration : IEntityTypeConfiguration<HotelProvider>
{
    public void Configure(EntityTypeBuilder<HotelProvider> builder)
    {
        builder.ToTable("hotel_provider");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.HotelId).HasColumnName("hotel_id");
        builder.Property(e => e.ProviderId).HasColumnName("provider_id");
        builder.Property(e => e.Enabled).HasColumnName("enabled").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasIndex(e => new { e.HotelId, e.ProviderId }).IsUnique();

        builder.HasOne(e => e.Hotel).WithMany(e => e.HotelProviders).HasForeignKey(e => e.HotelId);
        builder.HasOne(e => e.Provider).WithMany(e => e.HotelProviders).HasForeignKey(e => e.ProviderId);
        builder.HasMany(e => e.HotelProviderRoomTypes).WithOne(e => e.HotelProvider).HasForeignKey(e => e.HotelProviderId);
    }
}
