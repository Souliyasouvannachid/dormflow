namespace DormFlow.Contracts.Invoices;

/// <summary>ข้อมูลใบแจ้งหนี้หนึ่งฉบับ</summary>
public class InvoiceResponse
{
    /// <summary>รหัสอ้างอิงของใบแจ้งหนี้</summary>
    public Guid Id { get; init; }

    /// <summary>องค์กรที่เกี่ยวข้อง</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>หอพักที่เกี่ยวข้อง</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>สัญญาเช่าที่ใบแจ้งหนี้นี้อ้างอิงถึง</summary>
    public Guid ContractId { get; init; }

    /// <summary>เลขที่ใบแจ้งหนี้</summary>
    public string InvoiceNumber { get; init; } = string.Empty;

    /// <summary>เดือนที่ออกบิล</summary>
    public DateOnly PeriodMonth { get; init; }

    /// <summary>วันที่ออกใบแจ้งหนี้</summary>
    public DateOnly IssueDate { get; init; }

    /// <summary>วันที่ครบกำหนดชำระ</summary>
    public DateOnly DueDate { get; init; }

    /// <summary>ค่าเช่ารายเดือน</summary>
    public decimal RentAmount { get; init; }

    /// <summary>ค่าน้ำ</summary>
    public decimal WaterAmount { get; init; }

    /// <summary>ค่าไฟ</summary>
    public decimal ElectricityAmount { get; init; }

    /// <summary>ค่าปรับชำระล่าช้า</summary>
    public decimal LateFeeAmount { get; init; }

    /// <summary>ยอดรวมทั้งบิล</summary>
    public decimal TotalAmount { get; init; }

    /// <summary>ยอดที่ชำระแล้วสะสม</summary>
    public decimal PaidAmount { get; init; }

    /// <summary>สถานะของใบแจ้งหนี้</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; init; }

    /// <summary>เวลาที่สร้างข้อมูล</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
