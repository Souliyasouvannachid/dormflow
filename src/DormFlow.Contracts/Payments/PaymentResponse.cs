namespace DormFlow.Contracts.Payments;

/// <summary>ข้อมูลรายการชำระเงินหนึ่งรายการ</summary>
public class PaymentResponse
{
    /// <summary>รหัสอ้างอิงของรายการชำระเงิน</summary>
    public Guid Id { get; init; }

    /// <summary>องค์กรที่เกี่ยวข้อง</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>หอพักที่เกี่ยวข้อง</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ใบแจ้งหนี้ที่ชำระ</summary>
    public Guid InvoiceId { get; init; }

    /// <summary>จำนวนเงินที่ชำระ</summary>
    public decimal Amount { get; init; }

    /// <summary>วันเวลาที่ชำระเงิน</summary>
    public DateTimeOffset PaymentDate { get; init; }

    /// <summary>ช่องทางการชำระเงิน</summary>
    public string Method { get; init; } = string.Empty;

    /// <summary>เลขอ้างอิงการโอน/สลิป</summary>
    public string? ReferenceNumber { get; init; }

    /// <summary>สถานะของรายการชำระเงิน</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>เวลาที่ยืนยันรายการนี้</summary>
    public DateTimeOffset? ConfirmedAt { get; init; }

    /// <summary>เวลาที่คืนเงิน</summary>
    public DateTimeOffset? RefundedAt { get; init; }

    /// <summary>สาเหตุการคืนเงิน</summary>
    public string? RefundReason { get; init; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; init; }

    /// <summary>เวลาที่สร้างข้อมูล</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
