using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// รายการชำระเงินหนึ่งครั้งของใบแจ้งหนี้หนึ่งฉบับ
/// </summary>
/// <remarks>
/// บันทึกเป็น <see cref="PaymentStatus.Pending"/> ก่อนเสมอ ยังไม่มีผลต่อยอดใบแจ้งหนี้
/// จนกว่าจะถูกยืนยัน (<see cref="PaymentStatus.Confirmed"/>) ผ่านสิทธิ์ payments.approve
/// เพื่อให้มีจุดตรวจสอบก่อนเงินเข้าระบบจริง โดยเฉพาะกรณีโอนเงิน/QR ที่ต้องเทียบกับใบนำฝากธนาคาร
/// </remarks>
public class Payment : TenantEntity
{
    /// <summary>ใบแจ้งหนี้ที่ชำระ</summary>
    public Guid InvoiceId { get; set; }

    /// <summary>จำนวนเงินที่ชำระ</summary>
    public decimal Amount { get; set; }

    /// <summary>วันเวลาที่ชำระเงิน</summary>
    public DateTimeOffset PaymentDate { get; set; }

    /// <summary>ช่องทางการชำระเงิน</summary>
    public PaymentMethod Method { get; set; }

    /// <summary>เลขอ้างอิงการโอน/สลิป เป็น null เมื่อชำระด้วยเงินสด</summary>
    public string? ReferenceNumber { get; set; }

    /// <summary>สถานะของรายการชำระเงิน</summary>
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    /// <summary>เวลาที่ยืนยันรายการนี้ เป็น null เมื่อยังไม่ยืนยัน</summary>
    public DateTimeOffset? ConfirmedAt { get; set; }

    /// <summary>เวลาที่คืนเงิน เป็น null เมื่อยังไม่ถูกคืนเงิน</summary>
    public DateTimeOffset? RefundedAt { get; set; }

    /// <summary>สาเหตุการคืนเงิน</summary>
    public string? RefundReason { get; set; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
