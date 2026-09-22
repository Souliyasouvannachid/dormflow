using DormFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DormFlow.Infrastructure.Persistence.Configurations;

/// <summary>
/// ຕັ້ງຄ່າຕາຕະລາງຂອງ ASP.NET Core Identity
/// </summary>
/// <remarks>
/// ປ່ຽນຊື່ຕາຕະລາງຈາກຄ່າເລີ່ມຕົ້ນ AspNetUsers, AspNetRoles ໃຫ້ມັນເປັນ users, roles ອີງຕາມຂໍ້ຕົກລົງຂອງລະບົບ
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

        // ຕັ້ງຊື່ຖັນໃຫ້ກົງກັບສົນທິສັນຍາລະບົບແທນຊື່ phone_number ທີ່ Identity ໃຊ້ເປັນຄ່າເລີ່ມຕົ້ນ
        builder.Property(user => user.PhoneNumber).HasColumnName("phone").HasMaxLength(30);

        builder.HasIndex(user => user.OrganizationId);
        builder.HasIndex(user => user.Status);

        builder.HasOne<Domain.Entities.Organization>()
            .WithMany()
            .HasForeignKey(user => user.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>ຕັ້ງຄ່າຕາຕະລາງບົດບາດ</summary>
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

/// <summary>ຕັ້ງຄ່າຕາຕະລາງການພົວພັນຂອງຜູ້ໃຊ້</summary>
public sealed class IdentityUserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<Guid>>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("user_roles");
    }
}

/// <summary>ຕາຕະລາງຕັ້ງ Claim ຂອງຜູ້ໃຊ້</summary>
public sealed class IdentityUserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<Guid>>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<IdentityUserClaim<Guid>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("user_claims");
    }
}

/// <summary>ຕັ້ງຄ່າຕາຕະລາງການເຂົ້າສູ່ລະບົບກັບຜູ້ໃຫ້ບໍລິການພາຍນອກ</summary>
public sealed class IdentityUserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<Guid>>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("user_logins");
    }
}

/// <summary>ຕັ້ງຄ່າຕາຕະລາງຂອງ tokens ຜູ້ໃຊ້, ເຊັ່ນ: tokens reset ລະຫັດຜ່ານ</summary>
public sealed class IdentityUserTokenConfiguration : IEntityTypeConfiguration<IdentityUserToken<Guid>>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<IdentityUserToken<Guid>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("user_tokens");
    }
}

/// <summary>ຕາຕະລາງຕັ້ງ Claim ບົດບາດ</summary>
public sealed class IdentityRoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("role_claims");
    }
}
