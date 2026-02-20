using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("review");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.ReservationId).HasColumnName("reservation_id");
        builder.Property(e => e.UserId).HasColumnName("user_id");
        builder.Property(e => e.HotelId).HasColumnName("hotel_id");
        builder.Property(e => e.Rating).HasColumnName("rating");
        builder.Property(e => e.Title).HasColumnName("title").HasMaxLength(200);
        builder.Property(e => e.Comment).HasColumnName("comment").HasColumnType("text");
        builder.Property(e => e.Visible).HasColumnName("visible").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();

        builder.HasIndex(e => e.ReservationId).IsUnique();

        builder.HasOne(e => e.Reservation).WithOne(e => e.Review).HasForeignKey<Review>(e => e.ReservationId);
        builder.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
        builder.HasOne(e => e.Hotel).WithMany(e => e.Reviews).HasForeignKey(e => e.HotelId);
        builder.HasOne(e => e.ReviewResponse).WithOne(e => e.Review).HasForeignKey<ReviewResponse>(e => e.ReviewId);
    }
}
