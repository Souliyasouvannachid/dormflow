namespace DormFlow.Contracts.Deposits;

/// <summary>ການຮ້ອງຂໍເງິນຄືນເງິນຝາກ</summary>
public class RefundDepositRequest
{
    /// <summary>ຈຳນວນເງິນທີ່ຈະຄືນຕ້ອງຢູ່ລະຫວ່າງ 0 ກັບຈໍານວນເງິນຝາກທີ່ໄດ້ຮັບ (0 ໝາຍເຖິງການຍຶດທັງໝົດ)</summary>
    public decimal RefundAmount { get; set; }

    /// <summary>ເຫດຜົນສໍາລັບສ່ວນທີ່ຖືກຫັກບໍ່ຖືກສົ່ງຄືນ ຕ້ອງການເວລາກາງຄືນບໍ່ເຕັມທີ່</summary>
    public string? DeductionReason { get; set; }
}
