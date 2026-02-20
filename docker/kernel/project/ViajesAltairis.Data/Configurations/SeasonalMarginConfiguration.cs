using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class SeasonalMarginConfiguration : IEntityTypeConfiguration<SeasonalMargin>
{
    public void Configure(EntityTypeBuilder<SeasonalMargin> builder)
    {
        builder.ToTable("seasonal_margin");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.AdministrativeDivisionId).HasColumnName("administrative_division_id").IsRequired();
        builder.Property(e => e.StartMonthDay).HasColumnName("start_month_day").HasColumnType("char(5)").IsRequired();
        builder.Property(e => e.EndMonthDay).HasColumnName("end_month_day").HasColumnType("char(5)").IsRequired();
        builder.Property(e => e.Margin).HasColumnName("margin").HasColumnType("decimal(5,2)").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();

        builder.HasOne(e => e.AdministrativeDivision)
            .WithMany(e => e.SeasonalMargins)
            .HasForeignKey(e => e.AdministrativeDivisionId);
    }
}
