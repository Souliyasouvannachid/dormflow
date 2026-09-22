using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DormFlow.Infrastructure.Persistence.Configurations;

/// <summary>ຕັ້ງຄ່າຕາຕະລາງການເຮັດວຽກຂອງການແຈ້ງເຕືອນການບໍາລຸງຮັກສາ</summary>
public sealed class MaintenanceRequestConfiguration : IEntityTypeConfiguration<MaintenanceRequest>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<MaintenanceRequest> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("maintenance_requests");

        builder.Property(request => request.DormitoryId).IsRequired();
        builder.Property(request => request.Title).HasMaxLength(200).IsRequired();
        builder.Property(request => request.Description).HasMaxLength(2000);
        builder.Property(request => request.Priority).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(request => request.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(request => request.ResolutionNotes).HasMaxLength(2000);

        builder.HasIndex(request => request.RoomId);
        builder.HasIndex(request => request.Status);
        builder.HasIndex(request => request.AssignedTechnicianId);

        builder.HasOne<Room>()
            .WithMany()
            .HasForeignKey(request => request.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(request => request.AssignedTechnicianId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
