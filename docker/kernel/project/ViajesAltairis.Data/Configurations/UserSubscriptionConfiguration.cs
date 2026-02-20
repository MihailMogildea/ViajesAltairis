using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configurations;

public class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder.ToTable("user_subscription");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(e => e.SubscriptionTypeId).HasColumnName("subscription_type_id").IsRequired();
        builder.Property(e => e.StartDate).HasColumnName("start_date").IsRequired();
        builder.Property(e => e.EndDate).HasColumnName("end_date");
        builder.Property(e => e.Active).HasColumnName("active").HasColumnType("tinyint(1)").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();

        builder.HasOne(e => e.User)
            .WithMany(e => e.UserSubscriptions)
            .HasForeignKey(e => e.UserId);

        builder.HasOne(e => e.SubscriptionType)
            .WithMany(e => e.UserSubscriptions)
            .HasForeignKey(e => e.SubscriptionTypeId);
    }
}
