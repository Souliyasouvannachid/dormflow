using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ຜູ້ເຊົ່າຄົນໜຶ່ງ
/// </summary>
/// <remarks>
/// ສືບທອດມາຈາກ <see cref="BaseEntity"/> ບໍ່ <see cref="TenantEntity"/>
/// ເນື່ອງຈາກວ່າຜູ້ເຊົ່າບໍ່ໄດ້ຜູກມັດຢ່າງຖາວອນກັບຫໍພັກໃດນຶ່ງ. ໃນເວລາທີ່ແຕກຕ່າງກັນມັນເປັນໄປໄດ້ທີ່ຈະເຊົ່າຫໍພັກທີ່ແຕກຕ່າງກັນໃນອົງກອນດຽວກັນ
/// ຄວາມສຳພັນກັບຫໍພັກທີ່ເຈົ້າພັກຢູ່ສາມາດເຫັນໄດ້ຈາກ <see cref="Contract"/> ຍັງ Active ເທົ່ານັ້ນ
/// </remarks>
public class Tenant : BaseEntity
{
    /// <summary>ອົງກອນທີ່ຜູ້ເຊົ່ານີ້ຂຶ້ນກັບ</summary>
    public Guid OrganizationId { get; set; }

    /// <summary>ຊື່</summary>
    public required string FirstName { get; set; }

    /// <summary>ນາມສະກຸນ</summary>
    public required string LastName { get; set; }

    /// <summary>ອີເມວ</summary>
    public string? Email { get; set; }

    /// <summary>ເບີໂທລະສັບ</summary>
    public string? Phone { get; set; }

    /// <summary>ບັດປະຈຳຕົວ ຫຼືໝາຍເລກໜັງສືຜ່ານແດນ</summary>
    public string? NationalId { get; set; }

    /// <summary>ວັນເດືອນປີເກີດ</summary>
    public DateOnly? DateOfBirth { get; set; }

    /// <summary>ທີ່ຢູ່ ຕາມທະບຽນເຮືອນ</summary>
    public string? Address { get; set; }

    /// <summary>ຊື່ຂອງຜູ້ຕິດຕໍ່ສຸກເສີນ</summary>
    public string? EmergencyContactName { get; set; }

    /// <summary>ເບີໂທລະສັບຕິດຕໍ່ສຸກເສີນ</summary>
    public string? EmergencyContactPhone { get; set; }

    /// <summary>ສະຖານະຜູ້ເຊົ່າ</summary>
    public TenantStatus Status { get; set; } = TenantStatus.Active;

    /// <summary>ຊື່ເຕັມສໍາລັບການສະແດງ</summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
}
