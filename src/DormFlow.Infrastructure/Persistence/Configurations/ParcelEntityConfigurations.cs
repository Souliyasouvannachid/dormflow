using DormFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DormFlow.Infrastructure.Persistence.Configurations;

/// <summary>ຕັ້ງຄ່າຕາຕະລາງ parcel</summary>
public sealed class ParcelConfiguration : IEntityTypeConfiguration<Parcel>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Parcel> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("parcels");

        builder.Property(parcel => parcel.DormitoryId).IsRequired();
        builder.Property(parcel => parcel.CourierName).HasMaxLength(100);
        builder.Property(parcel => parcel.TrackingNumber).HasMaxLength(100);
        builder.Property(parcel => parcel.Description).HasMaxLength(500);
        builder.Property(parcel => parcel.ReceivedAt).IsRequired();
        builder.Property(parcel => parcel.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(parcel => parcel.PickedUpByName).HasMaxLength(200);
        builder.Property(parcel => parcel.Notes).HasMaxLength(1000);

        builder.HasIndex(parcel => parcel.RoomId);
        builder.HasIndex(parcel => parcel.TenantId);
        builder.HasIndex(parcel => parcel.Status);

        builder.HasOne<Room>()
            .WithMany()
            .HasForeignKey(parcel => parcel.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(parcel => parcel.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
