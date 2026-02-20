using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Data.Configuration;

public class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder.ToTable("user_subscription");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.UserId).HasColumnName("user_id");
        builder.Property(e => e.SubscriptionTypeId).HasColumnName("subscription_type_id");
        builder.Property(e => e.StartDate).HasColumnName("start_date");
        builder.Property(e => e.EndDate).HasColumnName("end_date");
        builder.Property(e => e.Active).HasColumnName("active").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();

        builder.HasOne(e => e.User).WithMany(e => e.UserSubscriptions).HasForeignKey(e => e.UserId);
        builder.HasOne(e => e.SubscriptionType).WithMany(e => e.UserSubscriptions).HasForeignKey(e => e.SubscriptionTypeId);
    }
}
