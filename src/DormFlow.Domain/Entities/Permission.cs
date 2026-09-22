using DormFlow.Domain.Common;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ໃບອະນຸຍາດຜຶ່ງ, e.g rooms.create
/// </summary>
/// <remarks>
/// ມັນເປັນການອ້າງອີງທີ່ລະບົບສ້າງສໍາລັບຕົວມັນເອງ Seed ມັນບໍ່ໃຫ້ຜູ້ໃຊ້ເພີ່ມມັນດ້ວຍຕົນເອງ
/// ເນື່ອງຈາກວ່າທຸກໆລະຫັດສິດພິເສດຕ້ອງມີຈຸດກວດກາຢູ່ໃນລະຫັດຕົວຈິງ
/// </remarks>
public class Permission : BaseEntity
{
    /// <summary>ລະຫັດການອະນຸຍາດທີ່ໃຊ້ສໍາລັບການອ້າງອີງໃນລະຫັດແລະໃນ Claim ຂອງ Access Token</summary>
    public required string Code { get; set; }

    /// <summary>ສະແດງຊື່ການອະນຸຍາດ</summary>
    public required string Name { get; set; }

    /// <summary>ຄໍາອະທິບາຍເພີ່ມເຕີມ</summary>
    public string? Description { get; set; }

    /// <summary>ໂມດູນທີ່ສິດອະນຸຍາດນີ້ແມ່ນໃຊ້ກັບກຸ່ມກຸ່ມໃນຜ້າຈໍການອະນຸຍາດ</summary>
    public required string Module { get; set; }

    /// <summary>ບົດບາດທີ່ມີສິດພິເສດນີ້</summary>
    public ICollection<RolePermission> RolePermissions { get; } = [];
}
