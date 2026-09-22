using DormFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DormFlow.Infrastructure.Persistence.Configurations;

/// <summary>ຕັ້ງຄ່າຕາຕະລາງໃບຮັບເງິນ</summary>
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

        // ໝາຍເລກໃບຮັບເງິນແມ່ນເປັນເອກະລັກພາຍໃນອົງກອນດຽວກັນ. ມີແຕ່ແຖວທີ່ຍັງບໍ່ໄດ້ລຶບ
        builder.HasIndex(receipt => new { receipt.OrganizationId, receipt.ReceiptNumber })
            .IsUnique()
            .HasFilter("is_deleted = false");

        // ສາມາດອອກໃບຮັບເງິນໄດ້ພຽງອັນດຽວສຳລັບທຸລະກຳການຈ່າຍເງິນອັນດຽວ
        builder.HasIndex(receipt => receipt.PaymentId)
            .IsUnique()
            .HasFilter("is_deleted = false");

        builder.HasOne<Payment>()
            .WithMany()
            .HasForeignKey(receipt => receipt.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
