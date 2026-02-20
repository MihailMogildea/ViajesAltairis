using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class BoardTypeConfiguration : IEntityTypeConfiguration<BoardType>
{
    public void Configure(EntityTypeBuilder<BoardType> builder)
    {
        builder.ToTable("board_type");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
    }
}
