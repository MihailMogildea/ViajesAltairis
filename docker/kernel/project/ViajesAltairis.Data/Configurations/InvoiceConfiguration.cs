using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("invoice");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.InvoiceNumber).HasColumnName("invoice_number").HasMaxLength(50).IsRequired();
        builder.Property(e => e.StatusId).HasColumnName("status_id").IsRequired();
        builder.Property(e => e.ReservationId).HasColumnName("reservation_id").IsRequired();
        builder.Property(e => e.BusinessPartnerId).HasColumnName("business_partner_id");
        builder.Property(e => e.Subtotal).HasColumnName("subtotal").HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(e => e.TaxAmount).HasColumnName("tax_amount").HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(e => e.DiscountAmount).HasColumnName("discount_amount").HasColumnType("decimal(10,2)").HasDefaultValue(0m);
        builder.Property(e => e.TotalAmount).HasColumnName("total_amount").HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(e => e.PeriodStart).HasColumnName("period_start").IsRequired();
        builder.Property(e => e.PeriodEnd).HasColumnName("period_end").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();

        builder.HasIndex(e => e.InvoiceNumber).IsUnique();

        builder.HasOne(e => e.Status)
            .WithMany(e => e.Invoices)
            .HasForeignKey(e => e.StatusId);

        builder.HasOne(e => e.Reservation)
            .WithMany(e => e.Invoices)
            .HasForeignKey(e => e.ReservationId);

        builder.HasOne(e => e.BusinessPartner)
            .WithMany(e => e.Invoices)
            .HasForeignKey(e => e.BusinessPartnerId);
    }
}
