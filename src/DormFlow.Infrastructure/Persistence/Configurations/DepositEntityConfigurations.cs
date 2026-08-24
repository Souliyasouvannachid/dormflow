using DormFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DormFlow.Infrastructure.Persistence.Configurations;

/// <summary>ตั้งค่าตารางเงินมัดจำ</summary>
public sealed class DepositConfiguration : IEntityTypeConfiguration<Deposit>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Deposit> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("deposits");

        builder.Property(deposit => deposit.DormitoryId).IsRequired();
        builder.Property(deposit => deposit.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(deposit => deposit.CollectedAt).IsRequired();
        builder.Property(deposit => deposit.DeductionReason).HasMaxLength(500);
        builder.Property(deposit => deposit.Notes).HasMaxLength(1000);

        // สัญญาเช่าหนึ่งฉบับรับเงินมัดจำได้เพียงรายการเดียว เฉพาะแถวที่ยังไม่ถูกลบ
        builder.HasIndex(deposit => deposit.ContractId)
            .IsUnique()
            .HasFilter("is_deleted = false");

        builder.HasIndex(deposit => deposit.Status);

        builder.HasOne<Contract>()
            .WithMany()
            .HasForeignKey(deposit => deposit.ContractId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
