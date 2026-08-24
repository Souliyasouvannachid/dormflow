using DormFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DormFlow.Infrastructure.Persistence.Configurations;

/// <summary>ตั้งค่าตารางค่ามิเตอร์</summary>
public sealed class MeterReadingConfiguration : IEntityTypeConfiguration<MeterReading>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<MeterReading> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("meter_readings");

        builder.Property(reading => reading.DormitoryId).IsRequired();
        builder.Property(reading => reading.MeterType).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(reading => reading.PeriodMonth).IsRequired();
        builder.Property(reading => reading.ReadingDate).IsRequired();
        builder.Property(reading => reading.Notes).HasMaxLength(1000);

        // ห้องหนึ่งห้องจดค่ามิเตอร์ประเภทเดียวกันได้เพียงครั้งเดียวต่อเดือน
        builder.HasIndex(reading => new { reading.RoomId, reading.MeterType, reading.PeriodMonth })
            .IsUnique()
            .HasFilter("is_deleted = false");

        builder.HasOne<Room>()
            .WithMany()
            .HasForeignKey(reading => reading.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
