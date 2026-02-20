using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
{
    public void Configure(EntityTypeBuilder<NotificationLog> builder)
    {
        builder.ToTable("notification_log");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.UserId).HasColumnName("user_id");
        builder.Property(e => e.EmailTemplateId).HasColumnName("email_template_id");
        builder.Property(e => e.RecipientEmail).HasColumnName("recipient_email").HasMaxLength(200);
        builder.Property(e => e.Subject).HasColumnName("subject").HasMaxLength(255);
        builder.Property(e => e.Body).HasColumnName("body").HasColumnType("text");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
        builder.HasOne(e => e.EmailTemplate).WithMany(e => e.NotificationLogs).HasForeignKey(e => e.EmailTemplateId);
    }
}
