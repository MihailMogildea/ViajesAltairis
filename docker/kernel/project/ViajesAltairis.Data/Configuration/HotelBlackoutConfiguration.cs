using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class HotelBlackoutConfiguration : IEntityTypeConfiguration<HotelBlackout>
{
    public void Configure(EntityTypeBuilder<HotelBlackout> builder)
    {
        builder.ToTable("hotel_blackout");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.HotelId).HasColumnName("hotel_id");
        builder.Property(e => e.StartDate).HasColumnName("start_date");
        builder.Property(e => e.EndDate).HasColumnName("end_date");
        builder.Property(e => e.Reason).HasColumnName("reason").HasMaxLength(255);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasOne(e => e.Hotel).WithMany(e => e.HotelBlackouts).HasForeignKey(e => e.HotelId);
    }
}
