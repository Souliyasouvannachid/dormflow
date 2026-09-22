using DormFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DormFlow.Infrastructure.Persistence.Configurations;

/// <summary>ຕັ້ງຄ່າຕາຕະລາງການຕັ້ງຄ່າ</summary>
public sealed class SettingConfiguration : IEntityTypeConfiguration<Setting>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("settings");

        builder.Property(setting => setting.Key).HasMaxLength(100).IsRequired();
        builder.Property(setting => setting.Value).HasMaxLength(2000).IsRequired();

        builder.HasIndex(setting => new { setting.OrganizationId, setting.Key })
            .IsUnique()
            .HasFilter("is_deleted = false");
    }
}
