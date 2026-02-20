using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class UserTypeConfiguration : IEntityTypeConfiguration<UserType>
{
    public void Configure(EntityTypeBuilder<UserType> builder)
    {
        builder.ToTable("user_type");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(50);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasIndex(e => e.Name).IsUnique();

        builder.HasMany(e => e.Users).WithOne(e => e.UserType).HasForeignKey(e => e.UserTypeId);
    }
}
