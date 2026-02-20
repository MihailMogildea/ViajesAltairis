using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class BusinessPartnerConfiguration : IEntityTypeConfiguration<BusinessPartner>
{
    public void Configure(EntityTypeBuilder<BusinessPartner> builder)
    {
        builder.ToTable("business_partner");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CompanyName).HasColumnName("company_name").HasMaxLength(200);
        builder.Property(e => e.TaxId).HasColumnName("tax_id").HasMaxLength(50);
        builder.Property(e => e.Discount).HasColumnName("discount").HasColumnType("decimal(5,2)").HasDefaultValue(0.00m);
        builder.Property(e => e.Address).HasColumnName("address").HasMaxLength(300);
        builder.Property(e => e.City).HasColumnName("city").HasMaxLength(100);
        builder.Property(e => e.PostalCode).HasColumnName("postal_code").HasMaxLength(20);
        builder.Property(e => e.Country).HasColumnName("country").HasMaxLength(100);
        builder.Property(e => e.ContactEmail).HasColumnName("contact_email").HasMaxLength(200);
        builder.Property(e => e.ContactPhone).HasColumnName("contact_phone").HasMaxLength(50);
        builder.Property(e => e.Enabled).HasColumnName("enabled").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasMany(e => e.Users).WithOne(e => e.BusinessPartner).HasForeignKey(e => e.BusinessPartnerId);
    }
}
