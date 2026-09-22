using DormFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DormFlow.Infrastructure.Persistence.Configurations;

/// <summary>ຕັ້ງໂຕະແຂກ</summary>
public sealed class VisitorConfiguration : IEntityTypeConfiguration<Visitor>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Visitor> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("visitors");

        builder.Property(visitor => visitor.DormitoryId).IsRequired();
        builder.Property(visitor => visitor.VisitorName).HasMaxLength(200).IsRequired();
        builder.Property(visitor => visitor.VisitorPhone).HasMaxLength(30);
        builder.Property(visitor => visitor.IdNumber).HasMaxLength(20);
        builder.Property(visitor => visitor.Purpose).HasMaxLength(500);
        builder.Property(visitor => visitor.CheckInAt).IsRequired();
        builder.Property(visitor => visitor.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(visitor => visitor.Notes).HasMaxLength(1000);

        builder.HasIndex(visitor => visitor.RoomId);
        builder.HasIndex(visitor => visitor.TenantId);
        builder.HasIndex(visitor => visitor.Status);

        builder.HasOne<Room>()
            .WithMany()
            .HasForeignKey(visitor => visitor.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(visitor => visitor.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
