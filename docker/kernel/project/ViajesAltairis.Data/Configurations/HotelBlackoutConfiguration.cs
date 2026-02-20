using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class HotelBlackoutConfiguration : IEntityTypeConfiguration<HotelBlackout>
{
    public void Configure(EntityTypeBuilder<HotelBlackout> builder)
    {
        builder.ToTable("hotel_blackout");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.HotelId).HasColumnName("hotel_id").IsRequired();
        builder.Property(e => e.StartDate).HasColumnName("start_date").IsRequired();
        builder.Property(e => e.EndDate).HasColumnName("end_date").IsRequired();
        builder.Property(e => e.Reason).HasColumnName("reason").HasMaxLength(255);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(e => e.Hotel)
            .WithMany(e => e.HotelBlackouts)
            .HasForeignKey(e => e.HotelId);
    }
}
