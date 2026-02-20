using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class ReservationGuestConfiguration : IEntityTypeConfiguration<ReservationGuest>
{
    public void Configure(EntityTypeBuilder<ReservationGuest> builder)
    {
        builder.ToTable("reservation_guest");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.ReservationLineId).HasColumnName("reservation_line_id").IsRequired();
        builder.Property(e => e.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Email).HasColumnName("email").HasMaxLength(200);
        builder.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(50);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(e => e.ReservationLine)
            .WithMany(e => e.ReservationGuests)
            .HasForeignKey(e => e.ReservationLineId);
    }
}
