namespace DormFlow.Contracts.Deposits;

/// <summary>ข้อมูลเงินมัดจำหนึ่งรายการ</summary>
public class DepositResponse
{
    /// <summary>รหัสอ้างอิงของเงินมัดจำ</summary>
    public Guid Id { get; init; }

    /// <summary>องค์กรที่เกี่ยวข้อง</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>หอพักที่เกี่ยวข้อง</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>สัญญาเช่าที่เงินมัดจำนี้ผูกอยู่</summary>
    public Guid ContractId { get; init; }

    /// <summary>จำนวนเงินมัดจำที่รับไว้</summary>
    public decimal Amount { get; init; }

    /// <summary>สถานะของเงินมัดจำ</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>เวลาที่รับเงินมัดจำ</summary>
    public DateTimeOffset CollectedAt { get; init; }

    /// <summary>จำนวนเงินที่คืนแล้ว</summary>
    public decimal RefundedAmount { get; init; }

    /// <summary>เวลาที่คืนเงิน</summary>
    public DateTimeOffset? RefundedAt { get; init; }

    /// <summary>เหตุผลของส่วนที่หักไว้ไม่คืน</summary>
    public string? DeductionReason { get; init; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; init; }

    /// <summary>เวลาที่สร้างข้อมูล</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
