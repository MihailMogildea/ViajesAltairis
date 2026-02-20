using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
{
    public void Configure(EntityTypeBuilder<NotificationLog> builder)
    {
        builder.ToTable("notification_log");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(e => e.EmailTemplateId).HasColumnName("email_template_id").IsRequired();
        builder.Property(e => e.RecipientEmail).HasColumnName("recipient_email").HasMaxLength(200).IsRequired();
        builder.Property(e => e.Subject).HasColumnName("subject").HasMaxLength(255).IsRequired();
        builder.Property(e => e.Body).HasColumnName("body").HasColumnType("text").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId);

        builder.HasOne(e => e.EmailTemplate)
            .WithMany(e => e.NotificationLogs)
            .HasForeignKey(e => e.EmailTemplateId);
    }
}
