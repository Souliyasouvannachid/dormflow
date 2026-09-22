namespace DormFlow.Contracts.Invoices;

/// <summary>ການຮ້ອງຂໍໃບແຈ້ງໜີ້ໃໝ່ຢູ່ໃນສະຖານະສະບັບຮ່າງ</summary>
public class CreateInvoiceRequest
{
    /// <summary>ສັນຍາເຊົ່າທີ່ຕ້ອງຮຽກເກັບຕ້ອງຢູ່ໃນສະຖານະທີ່ເປີດໃຊ້ງານ (Active) ເທົ່ານັ້ນ</summary>
    public Guid ContractId { get; set; }

    /// <summary>
    /// ເດືອນທີ່ທ່ານຕ້ອງການອອກໃບບິນລະບົບຈະປັບເປັນວັນທີ 1 ຂອງເດືອນອັດຕະໂນມັດ
    /// ແລະດຶງຂໍ້ມູນຄ່າວັດແທກນ້ໍາ/ຄ່າໄຟຟ້າຂອງຫ້ອງໃນເດືອນນີ້ແມ່ນໃຊ້ໃນການຄິດໄລ່ຄ່ານໍ້າ-ແສງສະຫວ່າງສະຜອງໃຫ້ (ແມ່ນ 0 ຖ້າບໍ່ມີຄ່າແມັດສໍາລັບເດືອນນີ້)
    /// </summary>
    public DateOnly PeriodMonth { get; set; }

    /// <summary>ວັນທີອອກໃບແຈ້ງໜີ້ ຖ້າບໍ່ລະບຸ, ໃຫ້ໃຊ້ວັນທີປັດຈຸບັນ</summary>
    public DateOnly? IssueDate { get; set; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
