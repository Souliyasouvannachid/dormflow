namespace DormFlow.Contracts.Invoices;

/// <summary>ຂໍ້ມູນໃບແຈ້ງຜີ້</summary>
public class InvoiceResponse
{
    /// <summary>ລະຫັດອ້າງອີງໃບເກັບເງິນ</summary>
    public Guid Id { get; init; }

    /// <summary>ອົງກອນທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>ຫໍພັກທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ສັນຍາເຊົ່າທີ່ໃບເກັບເງິນນີ້ຝາຍເຖິງ</summary>
    public Guid ContractId { get; init; }

    /// <summary>ໝາຍເລກໃບເກັບເງິນ</summary>
    public string InvoiceNumber { get; init; } = string.Empty;

    /// <summary>ເດືອນຂອງໃບບິນ</summary>
    public DateOnly PeriodMonth { get; init; }

    /// <summary>ວັນທີອອກໃບແຈ້ງໜີ້</summary>
    public DateOnly IssueDate { get; init; }

    /// <summary>ວັນທີການຊໍາລະເງິນ</summary>
    public DateOnly DueDate { get; init; }

    /// <summary>ຄ່າເຊົ່າລາຍເດືອນ</summary>
    public decimal RentAmount { get; init; }

    /// <summary>ໃບບິນຄ່ານໍ້າ</summary>
    public decimal WaterAmount { get; init; }

    /// <summary>ໃບບິນຄ່າໄຟຟ້າ</summary>
    public decimal ElectricityAmount { get; init; }

    /// <summary>ການລົງໂທດການຈ່າຍເງິນຊ້າ</summary>
    public decimal LateFeeAmount { get; init; }

    /// <summary>ລວມທັງໃບບິນຄ່າ</summary>
    public decimal TotalAmount { get; init; }

    /// <summary>ທັງໝົດທີ່ຈ່າຍ ແລະສະສົມ</summary>
    public decimal PaidAmount { get; init; }

    /// <summary>ສະຖານະໃບແຈ້ງໜີ້</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; init; }

    /// <summary>ເວລາສ້າງຂໍ້ມູນ</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
