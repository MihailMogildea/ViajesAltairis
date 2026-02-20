using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class HotelImageConfiguration : IEntityTypeConfiguration<HotelImage>
{
    public void Configure(EntityTypeBuilder<HotelImage> builder)
    {
        builder.ToTable("hotel_image");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(e => e.HotelId).HasColumnName("hotel_id").IsRequired();
        builder.Property(e => e.Url).HasColumnName("url").HasMaxLength(500).IsRequired();
        builder.Property(e => e.AltText).HasColumnName("alt_text").HasMaxLength(200);
        builder.Property(e => e.SortOrder).HasColumnName("sort_order").HasDefaultValue(0);

        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(e => e.Hotel)
            .WithMany(e => e.HotelImages)
            .HasForeignKey(e => e.HotelId);
    }
}
