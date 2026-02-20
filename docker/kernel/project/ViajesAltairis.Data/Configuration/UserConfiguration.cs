using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.UserTypeId).HasColumnName("user_type_id");
        builder.Property(e => e.Email).HasColumnName("email").HasMaxLength(200);
        builder.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255);
        builder.Property(e => e.FirstName).HasColumnName("first_name").HasMaxLength(100);
        builder.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(100);
        builder.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(50);
        builder.Property(e => e.TaxId).HasColumnName("tax_id").HasMaxLength(50);
        builder.Property(e => e.Address).HasColumnName("address").HasMaxLength(300);
        builder.Property(e => e.City).HasColumnName("city").HasMaxLength(100);
        builder.Property(e => e.PostalCode).HasColumnName("postal_code").HasMaxLength(20);
        builder.Property(e => e.Country).HasColumnName("country").HasMaxLength(100);
        builder.Property(e => e.LanguageId).HasColumnName("language_id");
        builder.Property(e => e.BusinessPartnerId).HasColumnName("business_partner_id");
        builder.Property(e => e.ProviderId).HasColumnName("provider_id");
        builder.Property(e => e.Discount).HasColumnName("discount").HasColumnType("decimal(5,2)").HasDefaultValue(0.00m);
        builder.Property(e => e.Enabled).HasColumnName("enabled").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasIndex(e => e.Email).IsUnique();

        builder.HasOne(e => e.UserType).WithMany(e => e.Users).HasForeignKey(e => e.UserTypeId);
        builder.HasOne(e => e.Language).WithMany().HasForeignKey(e => e.LanguageId);
        builder.HasOne(e => e.BusinessPartner).WithMany(e => e.Users).HasForeignKey(e => e.BusinessPartnerId);
        builder.HasOne(e => e.Provider).WithMany(e => e.Users).HasForeignKey(e => e.ProviderId);
        builder.HasMany(e => e.UserHotels).WithOne(e => e.User).HasForeignKey(e => e.UserId);
        builder.HasMany(e => e.UserSubscriptions).WithOne(e => e.User).HasForeignKey(e => e.UserId);
    }
}
