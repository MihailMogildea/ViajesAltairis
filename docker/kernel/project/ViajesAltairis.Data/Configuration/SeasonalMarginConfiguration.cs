using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class SeasonalMarginConfiguration : IEntityTypeConfiguration<SeasonalMargin>
{
    public void Configure(EntityTypeBuilder<SeasonalMargin> builder)
    {
        builder.ToTable("seasonal_margin");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.AdministrativeDivisionId).HasColumnName("administrative_division_id");
        builder.Property(e => e.StartMonthDay).HasColumnName("start_month_day").HasMaxLength(5).IsFixedLength();
        builder.Property(e => e.EndMonthDay).HasColumnName("end_month_day").HasMaxLength(5).IsFixedLength();
        builder.Property(e => e.Margin).HasColumnName("margin").HasColumnType("decimal(5,2)");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();

        builder.HasOne(e => e.AdministrativeDivision).WithMany(e => e.SeasonalMargins).HasForeignKey(e => e.AdministrativeDivisionId);
    }
}
