using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ຫໍພັກແຫ່ງໜຶ່ງພາຍໃນອົງກອນ
/// </summary>
/// <remarks>
/// ສືບທອດມາຈາກ <see cref="BaseEntity"/> ບໍ່ <see cref="TenantEntity"/>
/// ເນື່ອງຈາກວ່າຫໍພັກແມ່ນຂອບເຂດຂອງ DormitoryId ດັ່ງນັ້ນ, ບໍ່ມີຫໍພັກຂອງແມ່ທີ່ຈະອ້າງເຖິງ
/// ຄືກັນກັບວ່າ <see cref="Organization"/> ບໍ່ໄດ້ສືບທອດ TenantEntity
/// </remarks>
public class Dormitory : BaseEntity
{
    /// <summary>ອົງກອນເຈົ້າຂອງຫໍພັກ</summary>
    public Guid OrganizationId { get; set; }

    /// <summary>ລະຫັດຫໍພັກແມ່ນເປັນເອກະລັກພາຍໃນອົງກອນດຽວກັນ</summary>
    public required string Code { get; set; }

    /// <summary>ຊື່ຫໍພັກ</summary>
    public required string Name { get; set; }

    /// <summary>ຄໍາອະທິບາຍເພີ່ມເຕີມ</summary>
    public string? Description { get; set; }

    /// <summary>ເບີໂທລະສັບຕິດຕໍ່</summary>
    public string? Phone { get; set; }

    /// <summary>ອີເມວຕິດຕໍ່</summary>
    public string? Email { get; set; }

    /// <summary>ທີ່ຢູ່</summary>
    public string? Address { get; set; }

    /// <summary>ແຂວງ</summary>
    public string? Province { get; set; }

    /// <summary>ເມືອງ/ພື້ນທີ່</summary>
    public string? District { get; set; }

    /// <summary>ເມືອງ/ເມືອງ</summary>
    public string? Subdistrict { get; set; }

    /// <summary>ລະຫັດໄປສະນີ</summary>
    public string? PostalCode { get; set; }

    /// <summary>ເສັ້ນຂະໜານ</summary>
    public decimal? Latitude { get; set; }

    /// <summary>ເສັ້ນແວງ</summary>
    public decimal? Longitude { get; set; }

    /// <summary>ວັນທີໃນແຕ່ລະເດືອນໃບແຈ້ງໜີ້ຖືກອອກ (1-28)</summary>
    public int BillingDay { get; set; } = 1;

    /// <summary>ວັນທີການຊໍາລະເງິນໃນແຕ່ລະເດືອນ (1-31)</summary>
    public int PaymentDueDay { get; set; } = 5;

    /// <summary>ຮູບແບບການຄິດໄລ່ຄ່າທຳນຽມການຈ່າຍເງິນຊ້າ</summary>
    public LateFeeType LateFeeType { get; set; } = LateFeeType.Fixed;

    /// <summary>ຈໍານວນການປັບໃໝການຊໍາລະຊ້າຕາມຮູບແບບທີ່ລະບຸໄວ້ໃນ <see cref="LateFeeType"/></summary>
    public decimal LateFeeAmount { get; set; }

    /// <summary>ອັດຕາຄ່ານ້ຳຕໍ່ຫົວໜ່ວຍ ໃຊ້ເພື່ອຄິດໄລ່ຄ່ານ້ຳໃນໃບແຈ້ງໜີ້ຈາກຄ່າແມັດ</summary>
    public decimal WaterRatePerUnit { get; set; }

    /// <summary>ອັດຕາຄ່າໄຟຟ້າຕໍ່ຫົວໜ່ວຍໃຊ້ເພື່ອຄິດໄລ່ຄ່າໄຟຟ້າໃນໃບແຈ້ງໜີ້ຈາກຄ່າແມັດ</summary>
    public decimal ElectricityRatePerUnit { get; set; }

    /// <summary>ສະຖານະຫໍພັກ</summary>
    public DormitoryStatus Status { get; set; } = DormitoryStatus.Active;
}
