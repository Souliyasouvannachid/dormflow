namespace DormFlow.Contracts.Receipts;

/// <summary>ข้อมูลใบเสร็จรับเงินหนึ่งฉบับ</summary>
public class ReceiptResponse
{
    /// <summary>รหัสอ้างอิงของใบเสร็จ</summary>
    public Guid Id { get; init; }

    /// <summary>องค์กรที่เกี่ยวข้อง</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>หอพักที่เกี่ยวข้อง</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>รายการชำระเงินที่ออกใบเสร็จนี้ให้</summary>
    public Guid PaymentId { get; init; }

    /// <summary>เลขที่ใบเสร็จ</summary>
    public string ReceiptNumber { get; init; } = string.Empty;

    /// <summary>จำนวนเงินตามใบเสร็จ</summary>
    public decimal Amount { get; init; }

    /// <summary>เวลาที่ออกใบเสร็จ</summary>
    public DateTimeOffset IssuedAt { get; init; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; init; }

    /// <summary>เวลาที่สร้างข้อมูล</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
