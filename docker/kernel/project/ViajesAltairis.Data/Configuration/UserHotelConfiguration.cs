using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class UserHotelConfiguration : IEntityTypeConfiguration<UserHotel>
{
    public void Configure(EntityTypeBuilder<UserHotel> builder)
    {
        builder.ToTable("user_hotel");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.UserId).HasColumnName("user_id");
        builder.Property(e => e.HotelId).HasColumnName("hotel_id");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasIndex(e => new { e.UserId, e.HotelId }).IsUnique();

        builder.HasOne(e => e.User).WithMany(e => e.UserHotels).HasForeignKey(e => e.UserId);
        builder.HasOne(e => e.Hotel).WithMany(e => e.UserHotels).HasForeignKey(e => e.HotelId);
    }
}
