using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class CancellationPolicyConfiguration : IEntityTypeConfiguration<CancellationPolicy>
{
    public void Configure(EntityTypeBuilder<CancellationPolicy> builder)
    {
        builder.ToTable("cancellation_policy");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.HotelId).HasColumnName("hotel_id").IsRequired();
        builder.Property(e => e.FreeCancellationHours).HasColumnName("free_cancellation_hours").HasDefaultValue(48);
        builder.Property(e => e.PenaltyPercentage).HasColumnName("penalty_percentage").HasColumnType("decimal(5,2)").HasDefaultValue(100m);
        builder.Property(e => e.Enabled).HasColumnName("enabled").HasColumnType("tinyint(1)").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(e => e.Hotel)
            .WithMany(e => e.CancellationPolicies)
            .HasForeignKey(e => e.HotelId);
    }
}
