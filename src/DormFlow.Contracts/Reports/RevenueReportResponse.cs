namespace DormFlow.Contracts.Reports;

/// <summary>ສະຫຼຸບລາຍຮັບສໍາລັບໄລຍະເວລາທີ່ກໍານົດໄວ້</summary>
public class RevenueReportResponse
{
    /// <summary>ເດືອນເລີ່ມຕົ້ນຂອງໄລຍະເວລາສະຫຼຸບ</summary>
    public DateOnly PeriodFrom { get; init; }

    /// <summary>ເດືອນສິ້ນສຸດຂອງໄລຍະເວລາສະຫຼຸບ</summary>
    public DateOnly PeriodTo { get; init; }

    /// <summary>ຈຳນວນໃບແຈ້ງໜີ້ທັງໝົດໃນຮອບເວລາ (ຍົກເລີກບໍ່ໄດ້ນັບ)</summary>
    public int InvoiceCount { get; init; }

    /// <summary>ຈໍານວນໃບເກັບເງິນທີ່ຈ່າຍເຕັມ</summary>
    public int PaidInvoiceCount { get; init; }

    /// <summary>
    /// ຈຳນວນໃບເກັບເງິນທີ່ໝົດກຳນົດໃນເວລາລາຍງານຖືກຮ້ອງຂໍ (ການຄິດໄລ່ສົດຈາກວັນທີຄົບກຳນົດທຽບກັບມື້ນີ້
    /// ເນື່ອງຈາກວ່າລະບົບຍັງບໍ່ທັນມີ Background Job ສະຖານທີ່ສະຖານະ Overdue ອັດຕະໂນມັດ)
    /// </summary>
    public int OverdueInvoiceCount { get; init; }

    /// <summary>ຈຳນວນໃບແຈ້ງໜີ້ທັງໝົດໃນຮອບເວລາ</summary>
    public decimal TotalInvoiced { get; init; }

    /// <summary>ຈໍານວນທັງຝົດຕົວຈິງເກັບກໍາໃນລະຫວ່າງ</summary>
    public decimal TotalCollected { get; init; }

    /// <summary>ຍອດຄົງຄ້າງທັງໝົດ (TotalInvoiced - TotalCollected)</summary>
    public decimal TotalOutstanding { get; init; }
}
