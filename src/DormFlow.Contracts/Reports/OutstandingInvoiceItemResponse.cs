namespace DormFlow.Contracts.Reports;

/// <summary>ໃບແຈ້ງໜີ້ທີ່ຍັງຄ້າງອັນໜຶ່ງ</summary>
public class OutstandingInvoiceItemResponse
{
    /// <summary>ລະຫັດອ້າງອີງໃບເກັບເງິນ</summary>
    public Guid InvoiceId { get; init; }

    /// <summary>ໝາຍເລກໃບເກັບເງິນ</summary>
    public string InvoiceNumber { get; init; } = string.Empty;

    /// <summary>ສັນຍາເຊົ່າທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid ContractId { get; init; }

    /// <summary>ຫໍພັກທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ວັນທີການຊໍາລະເງິນ</summary>
    public DateOnly DueDate { get; init; }

    /// <summary>ລວມທັງໃບບິນຄ່າ</summary>
    public decimal TotalAmount { get; init; }

    /// <summary>ຈ່າຍທັງໝົດ</summary>
    public decimal PaidAmount { get; init; }

    /// <summary>ຍອດຄົງເຫຼືອທີ່ຕ້ອງຈ່າຍ</summary>
    public decimal RemainingBalance { get; init; }

    /// <summary>ຈໍານວນມື້ທີ່ຜ່ານໄປສໍາລັບການຈ່າຍເງິນແມ່ນ 0 ໃນເວລາທີ່ມັນບໍ່ທັນກໍານົດ</summary>
    public int DaysOverdue { get; init; }
}
