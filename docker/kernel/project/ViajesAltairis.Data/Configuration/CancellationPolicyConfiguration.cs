using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class CancellationPolicyConfiguration : IEntityTypeConfiguration<CancellationPolicy>
{
    public void Configure(EntityTypeBuilder<CancellationPolicy> builder)
    {
        builder.ToTable("cancellation_policy");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.HotelId).HasColumnName("hotel_id");
        builder.Property(e => e.FreeCancellationHours).HasColumnName("free_cancellation_hours").HasDefaultValue(48);
        builder.Property(e => e.PenaltyPercentage).HasColumnName("penalty_percentage").HasColumnType("decimal(5,2)").HasDefaultValue(100.00m);
        builder.Property(e => e.Enabled).HasColumnName("enabled").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasOne(e => e.Hotel).WithMany(e => e.CancellationPolicies).HasForeignKey(e => e.HotelId);
    }
}
