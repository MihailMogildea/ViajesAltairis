using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class BoardTypeConfiguration : IEntityTypeConfiguration<BoardType>
{
    public void Configure(EntityTypeBuilder<BoardType> builder)
    {
        builder.ToTable("board_type");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(50);

        builder.HasMany(e => e.HotelProviderRoomTypeBoards).WithOne(e => e.BoardType).HasForeignKey(e => e.BoardTypeId);
        builder.HasMany(e => e.ReservationLines).WithOne(e => e.BoardType).HasForeignKey(e => e.BoardTypeId);
    }
}
