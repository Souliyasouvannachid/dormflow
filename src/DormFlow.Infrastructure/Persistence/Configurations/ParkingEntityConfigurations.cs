using DormFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DormFlow.Infrastructure.Persistence.Configurations;

/// <summary>ກໍານົດຕາຕະລາງການລົງທະບຽນບ່ອນຈອດລົດ</summary>
public sealed class ParkingRegistrationConfiguration : IEntityTypeConfiguration<ParkingRegistration>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ParkingRegistration> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("parking_registrations");

        builder.Property(registration => registration.DormitoryId).IsRequired();
        builder.Property(registration => registration.LicensePlate).HasMaxLength(20).IsRequired();
        builder.Property(registration => registration.VehicleType).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(registration => registration.Brand).HasMaxLength(100);
        builder.Property(registration => registration.Color).HasMaxLength(50);
        builder.Property(registration => registration.ParkingSpotLabel).HasMaxLength(50);
        builder.Property(registration => registration.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(registration => registration.RegisteredAt).IsRequired();
        builder.Property(registration => registration.Notes).HasMaxLength(1000);

        builder.HasIndex(registration => registration.RoomId);
        builder.HasIndex(registration => registration.TenantId);
        builder.HasIndex(registration => registration.LicensePlate);
        builder.HasIndex(registration => registration.Status);

        builder.HasOne<Room>()
            .WithMany()
            .HasForeignKey(registration => registration.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(registration => registration.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
