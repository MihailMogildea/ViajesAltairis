using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class ReviewResponseConfiguration : IEntityTypeConfiguration<ReviewResponse>
{
    public void Configure(EntityTypeBuilder<ReviewResponse> builder)
    {
        builder.ToTable("review_response");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.ReviewId).HasColumnName("review_id");
        builder.Property(e => e.UserId).HasColumnName("user_id");
        builder.Property(e => e.Comment).HasColumnName("comment").HasColumnType("text");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasIndex(e => e.ReviewId).IsUnique();

        builder.HasOne(e => e.Review).WithOne(e => e.ReviewResponse).HasForeignKey<ReviewResponse>(e => e.ReviewId);
        builder.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
    }
}
