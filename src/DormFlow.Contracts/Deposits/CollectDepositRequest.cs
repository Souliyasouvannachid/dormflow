namespace DormFlow.Contracts.Deposits;

/// <summary>ຮ້ອງຂໍບັນທຶກການຮັບເງິນຝາກ</summary>
public class CollectDepositRequest
{
    /// <summary>ສັນຍາເຊົ່າທີ່ໄດ້ຮັບເງິນຝາກ</summary>
    public Guid ContractId { get; set; }

    /// <summary>ຈໍານວນເງິນຝາກທີ່ໄດ້ຮັບ ຖ້າບໍ່ລະບຸ, ໃຫ້ໃຊ້ມູນຄ່າ DepositAmount ຂອງສັນຍາ</summary>
    public decimal? Amount { get; set; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
