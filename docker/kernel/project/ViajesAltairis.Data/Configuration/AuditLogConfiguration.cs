using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_log");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.UserId).HasColumnName("user_id");
        builder.Property(e => e.EntityType).HasColumnName("entity_type").HasMaxLength(50);
        builder.Property(e => e.EntityId).HasColumnName("entity_id");
        builder.Property(e => e.Action).HasColumnName("action").HasMaxLength(20);
        builder.Property(e => e.OldValues).HasColumnName("old_values").HasColumnType("json");
        builder.Property(e => e.NewValues).HasColumnName("new_values").HasColumnType("json");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
    }
}
