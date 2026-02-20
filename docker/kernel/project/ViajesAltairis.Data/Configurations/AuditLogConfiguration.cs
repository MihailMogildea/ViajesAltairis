using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_log");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.UserId).HasColumnName("user_id");
        builder.Property(e => e.EntityType).HasColumnName("entity_type").HasMaxLength(50).IsRequired();
        builder.Property(e => e.EntityId).HasColumnName("entity_id").IsRequired();
        builder.Property(e => e.Action).HasColumnName("action").HasMaxLength(20).IsRequired();
        builder.Property(e => e.OldValues).HasColumnName("old_values").HasColumnType("json");
        builder.Property(e => e.NewValues).HasColumnName("new_values").HasColumnType("json");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId);
    }
}
