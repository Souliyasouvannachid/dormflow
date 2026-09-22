using DormFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DormFlow.Infrastructure.Persistence.Configurations;

/// <summary>ຕັ້ງຄ່າຕາຕະລາງຄ່າແມັດ</summary>
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

        // ຫ້ອງຜຶ່ງສາມາດບັນທຶກປະເພດດຽວກັນຂອງການອ່ານແມັດພຽງແຕ່ຜຶ່ງຄັ້ງຕໍ່ເດືອນ
        builder.HasIndex(reading => new { reading.RoomId, reading.MeterType, reading.PeriodMonth })
            .IsUnique()
            .HasFilter("is_deleted = false");

        builder.HasOne<Room>()
            .WithMany()
            .HasForeignKey(reading => reading.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>ຕັ້ງຄ່າຕາຕະລາງໃບແຈ້ງໜີ້</summary>
public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("invoices");

        builder.Property(invoice => invoice.DormitoryId).IsRequired();
        builder.Property(invoice => invoice.InvoiceNumber).HasMaxLength(50).IsRequired();
        builder.Property(invoice => invoice.PeriodMonth).IsRequired();
        builder.Property(invoice => invoice.IssueDate).IsRequired();
        builder.Property(invoice => invoice.DueDate).IsRequired();
        builder.Property(invoice => invoice.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(invoice => invoice.Notes).HasMaxLength(1000);

        // ໝາຍເລກໃບແຈ້ງໜີ້ແມ່ນເປັນເອກະລັກພາຍໃນອົງກອນດຽວກັນ. ມີແຕ່ແຖວທີ່ຍັງບໍ່ໄດ້ລຶບ
        builder.HasIndex(invoice => new { invoice.OrganizationId, invoice.InvoiceNumber })
            .IsUnique()
            .HasFilter("is_deleted = false");

        // ສັນຍາເຊົ່າຜຶ່ງສາມາດຖືກເອີ້ນເກັບເງິນຄັ້ງດຽວສໍາລັບເດືອນດຽວກັນ
        builder.HasIndex(invoice => new { invoice.ContractId, invoice.PeriodMonth })
            .IsUnique()
            .HasFilter("is_deleted = false");

        builder.HasIndex(invoice => invoice.Status);

        builder.HasOne<Contract>()
            .WithMany()
            .HasForeignKey(invoice => invoice.ContractId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>ຕັ້ງຄ່າຕາຕະລາງການຈ່າຍເງິນ</summary>
public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("payments");

        builder.Property(payment => payment.DormitoryId).IsRequired();
        builder.Property(payment => payment.PaymentDate).IsRequired();
        builder.Property(payment => payment.Method).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(payment => payment.ReferenceNumber).HasMaxLength(100);
        builder.Property(payment => payment.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(payment => payment.RefundReason).HasMaxLength(500);
        builder.Property(payment => payment.Notes).HasMaxLength(1000);

        builder.HasIndex(payment => payment.InvoiceId);
        builder.HasIndex(payment => payment.Status);

        builder.HasOne<Invoice>()
            .WithMany()
            .HasForeignKey(payment => payment.InvoiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
