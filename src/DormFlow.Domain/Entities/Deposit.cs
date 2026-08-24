using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// เงินมัดจำของสัญญาเช่าหนึ่งฉบับ
/// </summary>
/// <remarks>
/// แยกจากโมดูล Payments โดยตั้งใจ เพราะเป็นกระแสเงินคนละก้อนกัน (เงินมัดจำเป็นเงินค้ำประกันที่ต้องคืน
/// เมื่อสิ้นสุดสัญญา ไม่ใช่รายรับตามรอบบิลเหมือน Invoice/Payment) สัญญาเช่าหนึ่งฉบับมีเงินมัดจำได้เพียงรายการเดียว
/// การคืนเงินทำได้ครั้งเดียวต่อรายการ (คืนเต็ม/บางส่วน/ริบทั้งหมด) ไม่รองรับการคืนเป็นหลายงวดเหมือน Payment
/// </remarks>
public class Deposit : TenantEntity
{
    /// <summary>สัญญาเช่าที่เงินมัดจำนี้ผูกอยู่</summary>
    public Guid ContractId { get; set; }

    /// <summary>จำนวนเงินมัดจำที่รับไว้</summary>
    public decimal Amount { get; set; }

    /// <summary>สถานะของเงินมัดจำ</summary>
    public DepositStatus Status { get; set; } = DepositStatus.Held;

    /// <summary>เวลาที่รับเงินมัดจำ</summary>
    public DateTimeOffset CollectedAt { get; set; }

    /// <summary>จำนวนเงินที่คืนแล้ว เป็น 0 จนกว่าจะมีการคืนเงิน</summary>
    public decimal RefundedAmount { get; set; }

    /// <summary>เวลาที่คืนเงิน เป็น null เมื่อยังไม่เคยคืน</summary>
    public DateTimeOffset? RefundedAt { get; set; }

    /// <summary>เหตุผลของส่วนที่หักไว้ไม่คืน (ค่าเสียหาย ค่าปรับ ฯลฯ)</summary>
    public string? DeductionReason { get; set; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
