using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class JobScheduleConfiguration : IEntityTypeConfiguration<JobSchedule>
{
    public void Configure(EntityTypeBuilder<JobSchedule> builder)
    {
        builder.ToTable("job_schedule");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.JobKey).HasColumnName("job_key").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.CronExpression).HasColumnName("cron_expression").HasMaxLength(50).IsRequired();
        builder.Property(e => e.Enabled).HasColumnName("enabled").HasColumnType("tinyint(1)").HasDefaultValue(true);
        builder.Property(e => e.LastExecutedAt).HasColumnName("last_executed_at").HasColumnType("timestamp");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(e => e.JobKey).IsUnique();
    }
}
