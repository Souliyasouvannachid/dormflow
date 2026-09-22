using DormFlow.Domain.Enums;

namespace DormFlow.Application.Common.Models;

/// <summary>
/// ຂໍ້ມູນຜູ້ໃຊ້ທີ່ຈໍາເປັນເພື່ອອອກ tokens ແລະຕອບສະຜອງຂໍ້ມູນຜູ້ໃຊ້ໃນປະຈຸບັນ
/// </summary>
/// <remarks>
/// ແຍກອອກຈາກ Entity ຂອງ Identity ເພື່ອບໍ່ໃຫ້ຊັ້ນ Application ຕ້ອງຮູ້ ASP.NET Core Identity
/// ແລະເພື່ອປ້ອງກັນບໍ່ໃຫ້ມີຄຸນຄ່າເຊັ່ນ: ລະຫັດຜ່ານ hashes ຈາກການຮົ່ວໄຫຼ Infrastructure
/// </remarks>
public sealed record UserSecurityProfile
{
    /// <summary>ID ຜູ້ໃຊ້</summary>
    public required Guid UserId { get; init; }

    /// <summary>ອີເມວ</summary>
    public required string Email { get; init; }

    /// <summary>ຊື່</summary>
    public required string FirstName { get; init; }

    /// <summary>ນາມສະກຸນ</summary>
    public required string LastName { get; init; }

    /// <summary>ເບີໂທລະສັບ</summary>
    public string? Phone { get; init; }

    /// <summary>ທີ່ຢູ່ຮູບໂປຣໄຟລ໌</summary>
    public string? ProfileImage { get; init; }

    /// <summary>ອົງກອນທີ່ກ່ຽວຂ້ອງແມ່ນ null ສໍາລັບຜູ້ບໍລິຫານທີ່ບໍ່ຕິດກັບອົງກອນໃດໆ</summary>
    public Guid? OrganizationId { get; init; }

    /// <summary>ສະຖານະບັນຊີ</summary>
    public required UserStatus Status { get; init; }

    /// <summary>ເຂົ້າສູ່ລະບົບຄັ້ງສຸດທ້າຍ</summary>
    public DateTimeOffset? LastLoginAt { get; init; }

    /// <summary>ບົດບາດຂອງຜູ້ໃຊ້</summary>
    public IReadOnlyList<string> Roles { get; init; } = [];

    /// <summary>ລະຫັດການອະນຸຍາດທັງໝົດທີ່ຜູ້ໃຊ້ໄດ້ຮັບຈາກທຸກບົດບາດລວມກັນ</summary>
    public IReadOnlyList<string> Permissions { get; init; } = [];

    /// <summary>ຫໍພັກທີ່ຜູ້ໃຊ້ຖືກມອບຝາຍ</summary>
    public IReadOnlyList<Guid> DormitoryIds { get; init; } = [];

    /// <summary>ລາຍລະອຽດການມອບຝາຍຫໍພັກຜູ້ໃຊ້ນໍາໃຊ້ເພື່ອສະແດງຜົນໄດ້ຮັບໃນຜ້າຈໍການບໍລິຫານຜູ້ໃຊ້</summary>
    public IReadOnlyList<UserDormitoryAssignmentInfo> DormitoryAssignments { get; init; } = [];

    /// <summary>ເວລາສ້າງບັນຊີ</summary>
    public DateTimeOffset CreatedAt { get; init; }
}

/// <summary>ລາຍລະອຽດຂອງການມອບຝາຍຂອງຫໍພັກຜຶ່ງໃຫ້ກັບຜູ້ໃຊ້</summary>
/// <param name="DormitoryId">ຫໍພັກທີ່ຖືກມອບໝາຍ</param>
/// <param name="IsPrimary">ນີ້ແມ່ນຫໍພັກຫຼັກຂອງຜູ້ໃຊ້ບໍ?</param>
/// <param name="AssignedAt">ເວລາມອບໝາຍ</param>
public sealed record UserDormitoryAssignmentInfo(Guid DormitoryId, bool IsPrimary, DateTimeOffset AssignedAt);
