namespace DormFlow.Application.Common.Abstractions;

/// <summary>
/// ຂໍ້ມູນຜູ້ໃຊ້ຂອງຄໍາຮ້ອງຂໍໃນປະຈຸບັນແມ່ນອ່ານຈາກ Claim ຂອງ JWT
/// ໃຊ້ມັນເປັນແຫຼ່ງດຽວຂອງຄວາມຈິງສໍາລັບການກັ່ນຕອງຂໍ້ມູນໂດຍອົງກອນແລະຫໍພັກ
/// </summary>
public interface ICurrentUser
{
    /// <summary>User ID ແມ່ນ null ໃນເວລາທີ່ຄໍາຮ້ອງຂໍຍັງບໍ່ທັນໄດ້ຮັບການຢັ້ງຢືນ</summary>
    Guid? UserId { get; }

    /// <summary>ອົງກອນທີ່ຜູ້ໃຊ້ຂຶ້ນກັບແມ່ນ null ສໍາລັບ SuperAdmin ທີ່ບໍ່ໄດ້ຕິດພັນກັບອົງກອນ</summary>
    Guid? OrganizationId { get; }

    /// <summary>ຜູ້ໃຊ້ໄດ້ຮັບການຢືນຢັນແລ້ວບໍ?</summary>
    bool IsAuthenticated { get; }

    /// <summary>ຜູ້ໃຊ້ແມ່ນ SuperAdmin ການເຂົ້າເຖິງຂໍ້ມູນທົ່ວອົງກອນເປັນໄປໄດ້ບໍ?</summary>
    bool IsSuperAdmin { get; }

    /// <summary>ລາຍຊື່ບົດບາດຂອງຜູ້ໃຊ້</summary>
    IReadOnlyCollection<string> Roles { get; }

    /// <summary>ບັນຊີລາຍຊື່ຂອງຫໍພັກທີ່ຜູ້ໃຊ້ຖືກມອບຝາຍໃຫ້ເບິ່ງແຍງ</summary>
    IReadOnlyCollection<Guid> DormitoryIds { get; }

    /// <summary>ບັນຊີລາຍຊື່ຂອງລະຫັດສິດພິເສດທີ່ຜູ້ໃຊ້ມີ, e.g rooms.create</summary>
    IReadOnlyCollection<string> Permissions { get; }

    /// <summary>ເລກ IP ຕົ້ນກໍາເນີດຂອງຄໍາຮ້ອງຂໍໄດ້ຖືກບັນທຶກໄວ້ໃນ Audit Log ແລະ Refresh Token</summary>
    string? IpAddress { get; }

    /// <summary>User Agent ຂອງການຮ້ອງຂໍ, ໃຊ້ມັນເພື່ອບັນທຶກໃນ Audit Log ແລະ Refresh Token</summary>
    string? UserAgent { get; }

    /// <summary>ເສັ້ນທາງຂອງຄໍາຮ້ອງຂໍປະຈຸບັນຖືກນໍາໃຊ້ເພື່ອບັນທຶກມັນຢູ່ໃນ Audit Log</summary>
    string? RequestPath { get; }

    /// <summary>ກວດເບິ່ງວ່າຜູ້ໃຊ້ມີການອະນຸຍາດທີ່ລະບຸ SuperAdmin ຖືວ່າເຈົ້າມີສິດສະເໝີ</summary>
    /// <param name="permissionCode">ລະຫັດການອະນຸຍາດເຊັ່ນ rooms.change-status</param>
    bool HasPermission(string permissionCode);

    /// <summary>ກວດເບິ່ງວ່າຜູ້ໃຊ້ສາມາດເຂົ້າເຖິງຂໍ້ມູນຂອງຫໍພັກທີ່ລະບຸ</summary>
    /// <param name="dormitoryId">ລະຫັດຫໍພັກ</param>
    bool CanAccessDormitory(Guid dormitoryId);
}
