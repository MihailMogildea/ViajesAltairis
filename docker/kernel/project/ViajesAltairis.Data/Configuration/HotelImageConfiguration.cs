using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class HotelImageConfiguration : IEntityTypeConfiguration<HotelImage>
{
    public void Configure(EntityTypeBuilder<HotelImage> builder)
    {
        builder.ToTable("hotel_image");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.HotelId).HasColumnName("hotel_id");
        builder.Property(e => e.Url).HasColumnName("url").HasMaxLength(500);
        builder.Property(e => e.AltText).HasColumnName("alt_text").HasMaxLength(200);
        builder.Property(e => e.SortOrder).HasColumnName("sort_order").HasDefaultValue(0);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasOne(e => e.Hotel).WithMany(e => e.HotelImages).HasForeignKey(e => e.HotelId);
    }
}
