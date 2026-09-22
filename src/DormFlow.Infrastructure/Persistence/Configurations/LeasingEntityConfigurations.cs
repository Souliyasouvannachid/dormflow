using DormFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DormFlow.Infrastructure.Persistence.Configurations;

/// <summary>ຕັ້ງໂຕະຜູ້ເຊົ່າ</summary>
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

        // ໝາຍເລກບັດປະຈຳຕົວແມ່ນເປັນເອກະລັກພາຍໃນອົງກອນດຽວກັນ. ພຽງແຕ່ແຖວທີ່ບໍ່ໄດ້ຖືກລຶບລ້າງແລະຄ່າທີ່ລະບຸ
        builder.HasIndex(tenant => new { tenant.OrganizationId, tenant.NationalId })
            .IsUnique()
            .HasFilter("is_deleted = false AND national_id IS NOT NULL");

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(tenant => tenant.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>ຕັ້ງຄ່າຕາຕະລາງການຈອງ</summary>
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
        // ໃຫ້ຖານຂໍ້ມູນເປັນຜູ້ປ້ອງກັນການຈອງຊ້ຳໃນກໍລະນີທີ່ request ເຂົ້າມາພ້ອມກັນ.
        builder.HasIndex(reservation => reservation.RoomId)
            .IsUnique()
            .HasDatabaseName("ix_reservations_one_pending_per_room")
            .HasFilter("status = 'Pending' AND is_deleted = false");
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

/// <summary>ຕັ້ງຄ່າຕາຕະລາງການເຊົ່າ</summary>
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

        // ຕົວເລກສັນຍາແມ່ນເປັນເອກະລັກພາຍໃນອົງກອນດຽວກັນ. ມີແຕ່ແຖວທີ່ຍັງບໍ່ໄດ້ລຶບ
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
