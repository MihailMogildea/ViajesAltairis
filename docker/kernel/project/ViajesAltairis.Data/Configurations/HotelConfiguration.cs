using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.ToTable("hotel");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.CityId).HasColumnName("city_id").IsRequired();
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.Stars).HasColumnName("stars").HasColumnType("tinyint").IsRequired();
        builder.Property(e => e.Address).HasColumnName("address").HasMaxLength(300).IsRequired();
        builder.Property(e => e.Email).HasColumnName("email").HasMaxLength(150);
        builder.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(50);
        builder.Property(e => e.CheckInTime).HasColumnName("check_in_time").HasColumnType("time").HasDefaultValue(new TimeOnly(15, 0));
        builder.Property(e => e.CheckOutTime).HasColumnName("check_out_time").HasColumnType("time").HasDefaultValue(new TimeOnly(11, 0));
        builder.Property(e => e.Latitude).HasColumnName("latitude").HasColumnType("decimal(10,7)");
        builder.Property(e => e.Longitude).HasColumnName("longitude").HasColumnType("decimal(10,7)");
        builder.Property(e => e.Margin).HasColumnName("margin").HasColumnType("decimal(5,2)").IsRequired().HasDefaultValue(0m);
        builder.Property(e => e.Enabled).HasColumnName("enabled").HasColumnType("tinyint(1)").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(e => e.City)
            .WithMany(e => e.Hotels)
            .HasForeignKey(e => e.CityId);
    }
}
