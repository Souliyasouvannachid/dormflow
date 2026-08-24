using DormFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DormFlow.Infrastructure.Persistence.Configurations;

/// <summary>ตั้งค่าตารางใบเสร็จรับเงิน</summary>
public sealed class ReceiptConfiguration : IEntityTypeConfiguration<Receipt>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Receipt> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("receipts");

        builder.Property(receipt => receipt.DormitoryId).IsRequired();
        builder.Property(receipt => receipt.ReceiptNumber).HasMaxLength(50).IsRequired();
        builder.Property(receipt => receipt.IssuedAt).IsRequired();
        builder.Property(receipt => receipt.Notes).HasMaxLength(1000);

        // เลขที่ใบเสร็จไม่ซ้ำกันภายในองค์กรเดียวกัน เฉพาะแถวที่ยังไม่ถูกลบ
        builder.HasIndex(receipt => new { receipt.OrganizationId, receipt.ReceiptNumber })
            .IsUnique()
            .HasFilter("is_deleted = false");

        // รายการชำระเงินหนึ่งรายการออกใบเสร็จได้เพียงครั้งเดียว
        builder.HasIndex(receipt => receipt.PaymentId)
            .IsUnique()
            .HasFilter("is_deleted = false");

        builder.HasOne<Payment>()
            .WithMany()
            .HasForeignKey(receipt => receipt.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
