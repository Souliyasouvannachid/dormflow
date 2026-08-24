using DormFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DormFlow.Infrastructure.Persistence.Configurations;

/// <summary>
/// ตั้งค่าตารางของ ASP.NET Core Identity
/// </summary>
/// <remarks>
/// เปลี่ยนชื่อตารางจากค่าเริ่มต้น AspNetUsers, AspNetRoles ให้เป็น users, roles ตามข้อตกลงของระบบ
/// </remarks>
public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("users");

        builder.Property(user => user.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(user => user.LastName).HasMaxLength(100).IsRequired();
        builder.Property(user => user.ProfileImage).HasMaxLength(512);
        builder.Property(user => user.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

        // ตั้งชื่อคอลัมน์ให้ตรงกับข้อตกลงของระบบแทนชื่อ phone_number ที่ Identity ใช้เป็นค่าเริ่มต้น
        builder.Property(user => user.PhoneNumber).HasColumnName("phone").HasMaxLength(30);

        builder.HasIndex(user => user.OrganizationId);
        builder.HasIndex(user => user.Status);

        builder.HasOne<Domain.Entities.Organization>()
            .WithMany()
            .HasForeignKey(user => user.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>ตั้งค่าตารางบทบาท</summary>
public sealed class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("roles");

        builder.Property(role => role.Description).HasMaxLength(256);
        builder.Property(role => role.IsSystemRole).HasDefaultValue(false);
    }
}

/// <summary>ตั้งค่าตารางความสัมพันธ์ระหว่างผู้ใช้กับบทบาท</summary>
public sealed class IdentityUserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<Guid>>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("user_roles");
    }
}

/// <summary>ตั้งค่าตาราง Claim ของผู้ใช้</summary>
public sealed class IdentityUserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<Guid>>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<IdentityUserClaim<Guid>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("user_claims");
    }
}

/// <summary>ตั้งค่าตารางการเข้าสู่ระบบด้วยผู้ให้บริการภายนอก</summary>
public sealed class IdentityUserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<Guid>>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("user_logins");
    }
}

/// <summary>ตั้งค่าตารางโทเคนของผู้ใช้ เช่น โทเคนสำหรับตั้งรหัสผ่านใหม่</summary>
public sealed class IdentityUserTokenConfiguration : IEntityTypeConfiguration<IdentityUserToken<Guid>>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<IdentityUserToken<Guid>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("user_tokens");
    }
}

/// <summary>ตั้งค่าตาราง Claim ของบทบาท</summary>
public sealed class IdentityRoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("role_claims");
    }
}
