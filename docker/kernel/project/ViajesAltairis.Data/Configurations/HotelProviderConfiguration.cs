using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class HotelProviderConfiguration : IEntityTypeConfiguration<HotelProvider>
{
    public void Configure(EntityTypeBuilder<HotelProvider> builder)
    {
        builder.ToTable("hotel_provider");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.HotelId).HasColumnName("hotel_id").IsRequired();
        builder.Property(e => e.ProviderId).HasColumnName("provider_id").IsRequired();
        builder.Property(e => e.Enabled).HasColumnName("enabled").HasColumnType("tinyint(1)").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(e => new { e.HotelId, e.ProviderId }).IsUnique();

        builder.HasOne(e => e.Hotel)
            .WithMany(e => e.HotelProviders)
            .HasForeignKey(e => e.HotelId);

        builder.HasOne(e => e.Provider)
            .WithMany(e => e.HotelProviders)
            .HasForeignKey(e => e.ProviderId);
    }
}
