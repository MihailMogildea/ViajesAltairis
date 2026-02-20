using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("review");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.ReservationId).HasColumnName("reservation_id").IsRequired();
        builder.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(e => e.HotelId).HasColumnName("hotel_id").IsRequired();
        builder.Property(e => e.Rating).HasColumnName("rating").IsRequired();
        builder.Property(e => e.Title).HasColumnName("title").HasMaxLength(200);
        builder.Property(e => e.Comment).HasColumnName("comment").HasColumnType("text");
        builder.Property(e => e.Visible).HasColumnName("visible").HasColumnType("tinyint(1)").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();

        builder.HasIndex(e => e.ReservationId).IsUnique();

        builder.HasOne(e => e.Reservation)
            .WithOne(e => e.Review)
            .HasForeignKey<Review>(e => e.ReservationId);

        builder.HasOne(e => e.User)
            .WithMany(e => e.Reviews)
            .HasForeignKey(e => e.UserId);

        builder.HasOne(e => e.Hotel)
            .WithMany(e => e.Reviews)
            .HasForeignKey(e => e.HotelId);
    }
}
