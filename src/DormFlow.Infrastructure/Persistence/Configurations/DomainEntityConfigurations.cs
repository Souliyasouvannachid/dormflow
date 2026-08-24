using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DormFlow.Infrastructure.Persistence.Configurations;

/// <summary>ตั้งค่าตารางองค์กร</summary>
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

        // รหัสองค์กรต้องไม่ซ้ำกันเฉพาะในกลุ่มที่ยังไม่ถูกลบ
        // เพื่อให้นำรหัสเดิมกลับมาใช้ได้หลังลบองค์กรทิ้ง
        builder.HasIndex(organization => organization.Code)
            .IsUnique()
            .HasFilter("is_deleted = false");
    }
}

/// <summary>ตั้งค่าตารางสิทธิ์</summary>
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

/// <summary>ตั้งค่าตารางความสัมพันธ์ระหว่างบทบาทกับสิทธิ์</summary>
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

        // ถอนบทบาททิ้งแล้วสิทธิ์ที่ผูกไว้ต้องหายไปด้วย ไม่เช่นนั้นจะเหลือแถวที่ชี้ไปยังบทบาทที่ไม่มีอยู่
        builder.HasOne<ApplicationRole>()
            .WithMany()
            .HasForeignKey(rolePermission => rolePermission.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(rolePermission => rolePermission.PermissionId);
    }
}

/// <summary>ตั้งค่าตาราง Refresh Token</summary>
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

        // ค้นหาโทเคนด้วยค่าแฮชในทุกครั้งที่มีการต่ออายุ จึงต้องมี index ที่ไม่ซ้ำกัน
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

/// <summary>ตั้งค่าตาราง Audit Log</summary>
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

        // เก็บค่าเดิมและค่าใหม่เป็น jsonb เพื่อให้ค้นหาภายในโครงสร้างข้อมูลได้จากฝั่งฐานข้อมูล
        builder.Property(log => log.OldValues).HasColumnType("jsonb");
        builder.Property(log => log.NewValues).HasColumnType("jsonb");

        builder.HasIndex(log => log.CreatedAt);
        builder.HasIndex(log => log.UserId);
        builder.HasIndex(log => new { log.EntityName, log.EntityId });
        builder.HasIndex(log => new { log.OrganizationId, log.DormitoryId });
    }
}
