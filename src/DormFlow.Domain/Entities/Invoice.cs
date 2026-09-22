using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ໃບແຈ້ງໜີ້ການເຊົ່າໃນຮອບບິນໜຶ່ງເດືອນ
/// </summary>
/// <remarks>
/// ລາຍການລາຍຈ່າຍເກັບຮັກສາໄວ້ເປັນພາກສະຜາມໂດຍກົງ (ເຊົ່າ/ໃບບິນຄ່ານໍ້າ/ໃບບິນຄ່າໄຟຟ້າ/ດີ) ແທນທີ່ຈະແຍກອອກເປັນຕາຕະລາງ Line Item
/// ເນື່ອງຈາກວ່າຈໍານວນຂອງປະເພດຄ່າໃຊ້ຈ່າຍແມ່ນຄົງທີ່ແລະຈໍາກັດໃນຂອບເຂດນີ້. ບໍ່ຈໍາເປັນຕ້ອງມີຈໍານວນລາຍການທີ່ບໍ່ຮູ້ຈັກລ່ວງຜ້າ
/// </remarks>
public class Invoice : TenantEntity
{
    /// <summary>ສັນຍາເຊົ່າທີ່ໃບເກັບເງິນນີ້ຝາຍເຖິງ</summary>
    public Guid ContractId { get; set; }

    /// <summary>ໝາຍເລກໃບແຈ້ງໜີ້ບໍ່ສະເພາະພາຍໃນອົງກອນດຽວກັນ</summary>
    public required string InvoiceNumber { get; set; }

    /// <summary>ເດືອນຂອງໃບບິນ ປັບເປັນວັນທີ 1 ປະຈຳເດືອນ</summary>
    public DateOnly PeriodMonth { get; set; }

    /// <summary>ວັນທີອອກໃບແຈ້ງໜີ້</summary>
    public DateOnly IssueDate { get; set; }

    /// <summary>ວັນທີການຊໍາລະເງິນ</summary>
    public DateOnly DueDate { get; set; }

    /// <summary>ຄ່າເຊົ່າລາຍເດືອນ ສຳເນົາມາຈາກສັນຍາເຊົ່າເມື່ອອອກໃບບິນ</summary>
    public decimal RentAmount { get; set; }

    /// <summary>ຄ່ານ້ຳແມ່ນຄິດໄລ່ຈາກຫົວໜ່ວຍທີ່ໃຊ້ໃນຄ່າແມັດສຳລັບຮອບໃບບິນນີ້ x ອັດຕາຄ່ານ້ຳໃນຫໍພັກ</summary>
    public decimal WaterAmount { get; set; }

    /// <summary>ຄ່າໄຟຟ້າແມ່ນຄິດໄລ່ຈາກຫົວໜ່ວຍທີ່ໃຊ້ໃນຄ່າແມັດຂອງຮອບບິນນີ້ x ອັດຕາຄ່າໄຟຟ້າໃນຫໍພັກ</summary>
    public decimal ElectricityAmount { get; set; }

    /// <summary>ການລົງໂທດການຈ່າຍເງິນຊ້າແມ່ນ 0 ຈົນກ່ວາຈະໄດ້ຮັບການຄິດໄລ່ໃນເວລາທີ່ການຈ່າຍເງິນຝົດກໍານົດ</summary>
    public decimal LateFeeAmount { get; set; }

    /// <summary>ລວມທັງໃບບິນຄ່າ (RentAmount + WaterAmount + ElectricityAmount + LateFeeAmount)</summary>
    public decimal TotalAmount { get; set; }

    /// <summary>ຈໍານວນທີ່ຈ່າຍແມ່ນສະສົມຈາກການຈ່າຍເງິນທີ່ຢືນຢັນທັງຝົດສໍາລັບໃບບິນນີ້</summary>
    public decimal PaidAmount { get; set; }

    /// <summary>ສະຖານະໃບແຈ້ງໜີ້</summary>
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
