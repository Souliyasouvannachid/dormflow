using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ປະກາດຫໍພັກໜຶ່ງ
/// </summary>
/// <remarks>
/// ຜູກມັດຢູ່ຫໍພັກໜຶ່ງສະເໝີ (DormitoryId ບັງຄັບ) ບໍ່ຮອງຮັບການປະກາດຂອງບໍລິສັດທີ່ສະແດງໃນເວລາດຽວກັນຢູ່ໃນຫໍພັກທັງໝົດ
/// ເພາະສະເປັກບໍ່ໄດ້ລະບຸລາຍລະອຽດຂອງໂມດູນນີ້ເລີຍ
/// </remarks>
public class Announcement : TenantEntity
{
    public string Priority { get; set; } = "Normal";
    public string[] TargetRoles { get; set; } = [];

    /// <summary>ຫົວຂໍ້ປະກາດ</summary>
    public required string Title { get; set; }

    /// <summary>ເນື້ອໃນປະກາດ</summary>
    public required string Content { get; set; }

    /// <summary>ສະຖານະປະກາດ</summary>
    public AnnouncementStatus Status { get; set; } = AnnouncementStatus.Draft;

    /// <summary>ເວລາເຜີຍແຜ່ແມ່ນ null ໃນເວລາທີ່ບໍ່ທັນໄດ້ຈັດພີມມາ</summary>
    public DateTimeOffset? PublishedAt { get; set; }

    /// <summary>ເວລາໝົດອາຍຸປະກາດແມ່ນ null ເມື່ອບໍ່ໄດ້ລະບຸ</summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
