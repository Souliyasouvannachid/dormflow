using DormFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DormFlow.Infrastructure.Persistence.Configurations;

/// <summary>ตั้งค่าตารางผู้เช่า</summary>
public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("tenants");

        builder.Property(tenant => tenant.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(tenant => tenant.LastName).HasMaxLength(100).IsRequired();
        builder.Property(tenant => tenant.Email).HasMaxLength(256);
        builder.Property(tenant => tenant.Phone).HasMaxLength(30);
        builder.Property(tenant => tenant.NationalId).HasMaxLength(20);
        builder.Property(tenant => tenant.Address).HasMaxLength(500);
        builder.Property(tenant => tenant.EmergencyContactName).HasMaxLength(200);
        builder.Property(tenant => tenant.EmergencyContactPhone).HasMaxLength(30);
        builder.Property(tenant => tenant.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.HasIndex(tenant => tenant.OrganizationId);
        builder.HasIndex(tenant => new { tenant.OrganizationId, tenant.Phone });

        // เลขบัตรประชาชนไม่ซ้ำกันภายในองค์กรเดียวกัน เฉพาะแถวที่ยังไม่ถูกลบและระบุค่าไว้
        builder.HasIndex(tenant => new { tenant.OrganizationId, tenant.NationalId })
            .IsUnique()
            .HasFilter("is_deleted = false AND national_id IS NOT NULL");

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(tenant => tenant.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>ตั้งค่าตารางการจองห้องพัก</summary>
public sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("reservations");

        builder.Property(reservation => reservation.DormitoryId).IsRequired();
        builder.Property(reservation => reservation.ReservationDate).IsRequired();
        builder.Property(reservation => reservation.ExpiresAt).IsRequired();
        builder.Property(reservation => reservation.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(reservation => reservation.Notes).HasMaxLength(1000);

        builder.HasIndex(reservation => reservation.RoomId);
        builder.HasIndex(reservation => reservation.TenantId);
        builder.HasIndex(reservation => reservation.Status);

        builder.HasOne<Room>()
            .WithMany()
            .HasForeignKey(reservation => reservation.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(reservation => reservation.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>ตั้งค่าตารางสัญญาเช่า</summary>
public sealed class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("contracts");

        builder.Property(contract => contract.DormitoryId).IsRequired();
        builder.Property(contract => contract.ContractNumber).HasMaxLength(50).IsRequired();
        builder.Property(contract => contract.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(contract => contract.TerminationReason).HasMaxLength(500);
        builder.Property(contract => contract.Notes).HasMaxLength(1000);

        // เลขที่สัญญาไม่ซ้ำกันภายในองค์กรเดียวกัน เฉพาะแถวที่ยังไม่ถูกลบ
        builder.HasIndex(contract => new { contract.OrganizationId, contract.ContractNumber })
            .IsUnique()
            .HasFilter("is_deleted = false");

        builder.HasIndex(contract => contract.RoomId);
        builder.HasIndex(contract => contract.TenantId);
        builder.HasIndex(contract => contract.Status);

        builder.HasOne<Room>()
            .WithMany()
            .HasForeignKey(contract => contract.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(contract => contract.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Reservation>()
            .WithMany()
            .HasForeignKey(contract => contract.ReservationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
