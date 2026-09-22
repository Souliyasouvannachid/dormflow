using DormFlow.Domain.Common;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ການແຈ້ງເຕືອນອັນໜຶ່ງຖືກສົ່ງໄປຫາຜູ້ໃຊ້ທ້ອງຖິ່ນຄົນໜຶ່ງ
/// </summary>
/// <remarks>
/// ບໍ່ໄດ້ສືບທອດມາຈາກ <see cref="TenantEntity"/> ເນື່ອງຈາກວ່າການແຈ້ງເຕືອນໄດ້ຖືກຜູກມັດໂດຍກົງກັບຜູ້ໃຊ້ຜູ້ຮັບ (RecipientUserId)
/// ມັນບໍ່ແມ່ນຂໍ້ມູນທີ່ຕ້ອງໄດ້ຮັບການກັ່ນຕອງຜ່ານຫໍພັກ List Endpoint ທົ່ວໄປ — ຜູ້ໃຊ້ທຸກຄົນສະເຝີເຫັນພຽງແຕ່ການແຈ້ງເຕືອນຂອງຕົນເອງ
/// </remarks>
public class Notification : BaseEntity
{
    /// <summary>ອົງກອນຜູ້ຮັບແມ່ນ null ໃນເວລາທີ່ຜູ້ຮັບແມ່ນ SuperAdmin ທີ່ບໍ່ໄດ້ຕິດພັນກັບອົງກອນ</summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>ຜູ້ໃຊ້ທີ່ເປັນຜູ້ຮັບການແຈ້ງເຕືອນນີ້</summary>
    public Guid RecipientUserId { get; set; }

    /// <summary>ຫົວຂໍ້ແຈ້ງການ</summary>
    public required string Title { get; set; }

    /// <summary>ເນື້ອໃນການແຈ້ງເຕືອນ</summary>
    public required string Message { get; set; }

    /// <summary>ເຈົ້າໄດ້ອ່ານແລ້ວບໍ?</summary>
    public bool IsRead { get; set; }

    /// <summary>ເວລາອ່ານແມ່ນ null ເມື່ອເຈົ້າຍັງບໍ່ໄດ້ອ່ານມັນເທື່ອ</summary>
    public DateTimeOffset? ReadAt { get; set; }
}
