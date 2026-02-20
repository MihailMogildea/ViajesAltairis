using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class RoomTypeConfiguration : IEntityTypeConfiguration<RoomType>
{
    public void Configure(EntityTypeBuilder<RoomType> builder)
    {
        builder.ToTable("room_type");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(50);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasIndex(e => e.Name).IsUnique();

        builder.HasMany(e => e.HotelProviderRoomTypes).WithOne(e => e.RoomType).HasForeignKey(e => e.RoomTypeId);
    }
}
