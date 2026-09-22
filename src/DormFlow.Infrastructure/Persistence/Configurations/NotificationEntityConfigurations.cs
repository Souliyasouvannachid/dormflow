using DormFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DormFlow.Infrastructure.Persistence.Configurations;

/// <summary>ຕັ້ງຄ່າກຳນົດເວລາການແຈ້ງເຕືອນ</summary>
public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("notifications");

        builder.Property(notification => notification.Title).HasMaxLength(200).IsRequired();
        builder.Property(notification => notification.Message).HasMaxLength(2000).IsRequired();
        builder.Property(notification => notification.IsRead).IsRequired();

        builder.HasIndex(notification => notification.RecipientUserId);
        builder.HasIndex(notification => new { notification.RecipientUserId, notification.IsRead });
    }
}
