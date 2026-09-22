using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DormFlow.Infrastructure.Persistence.Configurations;

/// <summary>ຕັ້ງຄ່າຕາຕະລາງອົງກອນ</summary>
public sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("organizations");

        builder.Property(organization => organization.Code).HasMaxLength(50).IsRequired();
        builder.Property(organization => organization.Name).HasMaxLength(200).IsRequired();
        builder.Property(organization => organization.TaxId).HasMaxLength(20);
        builder.Property(organization => organization.Phone).HasMaxLength(30);
        builder.Property(organization => organization.Email).HasMaxLength(256);
        builder.Property(organization => organization.Address).HasMaxLength(500);
        builder.Property(organization => organization.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

        // ID ອົງກອນຈະຕ້ອງເປັນເອກະລັກສະເພາະໃນບັນດາກຸ່ມທີ່ຍັງບໍ່ໄດ້ລຶບ
        // ເພື່ອໃຫ້ສາມາດໃຊ້ລະຫັດເກົ່າຄືນໃຝ່ຫຼັງຈາກລຶບອົງກອນ
        builder.HasIndex(organization => organization.Code)
            .IsUnique()
            .HasFilter("is_deleted = false");
    }
}

/// <summary>ຕັ້ງຄ່າຕາຕະລາງການອະນຸຍາດ</summary>
public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("permissions");

        builder.Property(permission => permission.Code).HasMaxLength(100).IsRequired();
        builder.Property(permission => permission.Name).HasMaxLength(200).IsRequired();
        builder.Property(permission => permission.Description).HasMaxLength(500);
        builder.Property(permission => permission.Module).HasMaxLength(100).IsRequired();

        builder.HasIndex(permission => permission.Code).IsUnique();
        builder.HasIndex(permission => permission.Module);
    }
}

/// <summary>ຕັ້ງຄ່າຕາຕະລາງຄວາມສໍາພັນຂອງບົດບາດການອະນຸຍາດ</summary>
public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("role_permissions");

        builder.HasKey(rolePermission => new { rolePermission.RoleId, rolePermission.PermissionId });

        builder.Property(rolePermission => rolePermission.GrantedAt).IsRequired();

        builder.HasOne(rolePermission => rolePermission.Permission)
            .WithMany(permission => permission.RolePermissions)
            .HasForeignKey(rolePermission => rolePermission.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        // ຖອດຖອນບົດບາດແລະສິດທີ່ຕິດພັນກັບມັນຍັງຕ້ອງຫາຍໄປ. ຖ້າບໍ່ດັ່ງນັ້ນ, ແຖວຈະຖືກປະໄວ້ໂດຍຊີ້ໄປຫາບົດບາດທີ່ບໍ່ມີຢູ່ແລ້ວ
        builder.HasOne<ApplicationRole>()
            .WithMany()
            .HasForeignKey(rolePermission => rolePermission.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(rolePermission => rolePermission.PermissionId);
    }
}

/// <summary>ຕາຕະລາງຕັ້ງ Refresh Token</summary>
public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("refresh_tokens");

        builder.Property(token => token.TokenHash).HasMaxLength(128).IsRequired();
        builder.Property(token => token.JwtId).HasMaxLength(64).IsRequired();
        builder.Property(token => token.CreatedByIp).HasMaxLength(64);
        builder.Property(token => token.RevokedByIp).HasMaxLength(64);
        builder.Property(token => token.UserAgent).HasMaxLength(512);
        builder.Property(token => token.ReasonRevoked).HasConversion<string>().HasMaxLength(40);

        // ຄົ້ນຫາ tokens ໂດຍຄ່າ hash ຂອງພວກເຂົາທຸກຄັ້ງທີ່ພວກມັນຖືກຕໍ່ໃຝ່, ດັ່ງນັ້ນຕ້ອງມີ index ເປັນເອກະລັກ
        builder.HasIndex(token => token.TokenHash).IsUnique();
        builder.HasIndex(token => new { token.UserId, token.ExpiresAt });

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(token => token.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<RefreshToken>()
            .WithMany()
            .HasForeignKey(token => token.ReplacedByTokenId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>ຕາຕະລາງຕັ້ງ Audit Log</summary>
public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("audit_logs");

        builder.HasKey(log => log.Id);

        builder.Property(log => log.Action).HasConversion<string>().HasMaxLength(40).IsRequired();
        builder.Property(log => log.EntityName).HasMaxLength(100).IsRequired();
        builder.Property(log => log.EntityId).HasMaxLength(64);
        builder.Property(log => log.IpAddress).HasMaxLength(64);
        builder.Property(log => log.UserAgent).HasMaxLength(512);
        builder.Property(log => log.RequestPath).HasMaxLength(512);
        builder.Property(log => log.CreatedAt).IsRequired();

        // ຮັກສາຄຸນຄ່າຕົ້ນສະບັບແລະໃຝ່ເປັນ jsonb ເພື່ອໃຫ້ສາມາດຄົ້ນຫາພາຍໃນໂຄງສ້າງຂໍ້ມູນຈາກດ້ານຖານຂໍ້ມູນ
        builder.Property(log => log.OldValues).HasColumnType("jsonb");
        builder.Property(log => log.NewValues).HasColumnType("jsonb");

        builder.HasIndex(log => log.CreatedAt);
        builder.HasIndex(log => log.UserId);
        builder.HasIndex(log => new { log.EntityName, log.EntityId });
        builder.HasIndex(log => new { log.OrganizationId, log.DormitoryId });
    }
}
