using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ຄ່າໃຊ້ຈ່າຍປະຕິບັດການຜຶ່ງ (ໃບບິນຄ່ານໍ້າ-ແສງກາງ, ຄ່າໃຊ້ຈ່າຍໃນການບໍາລຸງຮັກສາ, ເງິນເດືອນ ແລະ ອື່ນໆ)
/// </summary>
/// <remarks>
/// ມັນເປັນການບັນທຶກຄ່າໃຊ້ຈ່າຍຂອງຫໍພັກ, ແຍກອອກຈາກ Invoice/Payment ເຊິ່ງທັງໝົດແມ່ນຢູ່ໃນດ້ານລາຍຮັບຈາກຜູ້ເຊົ່າ
/// ບໍ່ມີສະຖານະບໍ? Workflow ອະນຸມັດເນື່ອງຈາກວ່າມັນເປັນພຽງແຕ່ປື້ມບັນທຶກຄ່າໃຊ້ຈ່າຍ. ມັນບໍ່ແມ່ນລາຍຮັບທີ່ຕ້ອງໄດ້ຮັບການຄືນດີກັບໃບເກັບເງິນໃດໆ
/// </remarks>
public class Expense : TenantEntity
{
    /// <summary>ປະເພດຂອງຄ່າໃຊ້ຈ່າຍ</summary>
    public ExpenseCategory Category { get; set; }

    /// <summary>ຈໍານວນເງິນ</summary>
    public decimal Amount { get; set; }

    /// <summary>ວັນທີຈ່າຍຄ່າໃຊ້ຈ່າຍ</summary>
    public DateOnly ExpenseDate { get; set; }

    /// <summary>ລາຍລະອຽດລາຍການ</summary>
    public required string Description { get; set; }

    /// <summary>ຜູ້ຮັບເງິນ/ຮ້ານ/ຜູ້ໃຫ້ບໍລິການ</summary>
    public string? PaidTo { get; set; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
