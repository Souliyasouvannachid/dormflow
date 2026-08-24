using DormFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DormFlow.Infrastructure.Persistence.Configurations;

/// <summary>ตั้งค่าตารางประกาศ</summary>
public sealed class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("announcements");

        builder.Property(announcement => announcement.DormitoryId).IsRequired();
        builder.Property(announcement => announcement.Title).HasMaxLength(200).IsRequired();
        builder.Property(announcement => announcement.Content).HasMaxLength(5000).IsRequired();
        builder.Property(announcement => announcement.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(announcement => announcement.Notes).HasMaxLength(1000);

        builder.HasIndex(announcement => announcement.DormitoryId);
        builder.HasIndex(announcement => announcement.Status);
    }
}
