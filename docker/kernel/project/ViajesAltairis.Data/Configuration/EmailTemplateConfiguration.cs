using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class EmailTemplateConfiguration : IEntityTypeConfiguration<EmailTemplate>
{
    public void Configure(EntityTypeBuilder<EmailTemplate> builder)
    {
        builder.ToTable("email_template");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(50);

        builder.HasIndex(e => e.Name).IsUnique();

        builder.HasMany(e => e.NotificationLogs).WithOne(e => e.EmailTemplate).HasForeignKey(e => e.EmailTemplateId);
    }
}
