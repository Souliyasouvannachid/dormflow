using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ອົງກອນທີ່ນໍາໃຊ້ລະບົບ ມັນເປັນຮາກຂອງການແຍກຂໍ້ມູນຫຼາຍຜູ້ເຊົ່າ (Tenant Root)
/// </summary>
/// <remarks>
/// ສືບທອດມາຈາກ <see cref="BaseEntity"/> ບໍ່ <see cref="TenantEntity"/>
/// ເນື່ອງຈາກວ່າອົງກອນແມ່ນຂອບເຂດຂອງມັນເອງ. ດັ່ງນັ້ນ, ບໍ່ມີອົງກອນແມ່ທີ່ຈະອ້າງເຖິງ
/// </remarks>
public class Organization : BaseEntity
{
    /// <summary>ລະຫັດອົງກອນແມ່ນເປັນເອກະລັກໃນທົ່ວລະບົບ</summary>
    public required string Code { get; set; }

    /// <summary>ຊື່ອົງກອນ</summary>
    public required string Name { get; set; }

    /// <summary>ໝາຍເລກປະຈຳຕົວຜູ້ເສຍພາສີ</summary>
    public string? TaxId { get; set; }

    /// <summary>ເບີໂທລະສັບຕິດຕໍ່</summary>
    public string? Phone { get; set; }

    /// <summary>ອີເມວຕິດຕໍ່</summary>
    public string? Email { get; set; }

    /// <summary>ທີ່ຢູ່</summary>
    public string? Address { get; set; }

    /// <summary>ສະຖານະອົງກອນ</summary>
    public OrganizationStatus Status { get; set; } = OrganizationStatus.Active;

    /// <summary>ອົງກອນຢູ່ໃນສະຖານະທີ່ຜູ້ໃຊ້ສາມາດເຂົ້າສູ່ລະບົບໄດ້ບໍ?</summary>
    public bool CanUsersSignIn => Status == OrganizationStatus.Active;
}
