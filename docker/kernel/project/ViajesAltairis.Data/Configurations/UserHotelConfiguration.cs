using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class UserHotelConfiguration : IEntityTypeConfiguration<UserHotel>
{
    public void Configure(EntityTypeBuilder<UserHotel> builder)
    {
        builder.ToTable("user_hotel");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(e => e.HotelId).HasColumnName("hotel_id").IsRequired();

        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(e => new { e.UserId, e.HotelId }).IsUnique();

        builder.HasOne(e => e.User)
            .WithMany(e => e.UserHotels)
            .HasForeignKey(e => e.UserId);

        builder.HasOne(e => e.Hotel)
            .WithMany(e => e.UserHotels)
            .HasForeignKey(e => e.HotelId);
    }
}
