using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DormFlow.Infrastructure.Identity;

/// <summary>
/// ບັນຊີຜູ້ໃຊ້ລະບົບ, ສ້າງຂຶ້ນຈາກ <see cref="IdentityUser{TKey}"/> ຂອງ ASP.NET Core Identity
/// </summary>
/// <remarks>
/// ໃນຫ້ອງຮຽນ Infrastructure ເນື່ອງຈາກວ່າມັນໄດ້ຖືກຜູກມັດກັບ ASP.NET Core Identity ໂດຍກົງ
/// ຊັ້ນ Application ເບິ່ງເຫັນໄດ້ຕໍ່ກັບຜູ້ໃຊ້ UserSecurityProfile ພຽງແຕ່ຫຼັງຈາກນັ້ນບໍ່ມີວິທີທີ່ຈະແຕະ hash ລະຫັດຜ່ານ
/// ປະກາດ <see cref="IAuditableEntity"/> ແລະ <see cref="ISoftDeletable"/>
/// ເຮັດໃຫ້ຜູ້ໃຊ້ມີຂໍ້ມູນ Audit ແລະ Global Query Filter ເຊັ່ນດຽວກັນກັບ Entity ອື່ນໆທັງໝົດ
/// </remarks>
public class ApplicationUser : IdentityUser<Guid>, IAuditableEntity, ISoftDeletable
{
    /// <summary>ຊື່</summary>
    public required string FirstName { get; set; }

    /// <summary>ນາມສະກຸນ</summary>
    public required string LastName { get; set; }

    /// <summary>ທີ່ຢູ່ຮູບໂປຣໄຟລ໌</summary>
    public string? ProfileImage { get; set; }

    /// <summary>ອົງກອນທີ່ກ່ຽວຂ້ອງແມ່ນ null ສໍາລັບຜູ້ບໍລິຫານທີ່ຄຸ້ມຄອງໃນທົ່ວອົງກອນ</summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>ສະຖານະບັນຊີ</summary>
    public UserStatus Status { get; set; } = UserStatus.Active;

    /// <summary>ເວລາຂອງການເຂົ້າສູ່ລະບົບສົບຜົນສໍາເລັດຄັ້ງສຸດທ້າຍ</summary>
    public DateTimeOffset? LastLoginAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; set; }

    /// <inheritdoc />
    public Guid? CreatedBy { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <inheritdoc />
    public Guid? UpdatedBy { get; set; }

    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? DeletedAt { get; set; }

    /// <inheritdoc />
    public Guid? DeletedBy { get; set; }

    /// <summary>ຊື່ເຕັມສໍາລັບການສະແດງ</summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>ບັນຊີຢູ່ໃນສະຖານະທີ່ເຂົ້າສູ່ລະບົບບໍ?</summary>
    public bool CanSignIn => !IsDeleted && Status == UserStatus.Active;
}
