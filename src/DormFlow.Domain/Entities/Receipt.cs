using DormFlow.Domain.Common;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ใบเสร็จรับเงินของรายการชำระเงินหนึ่งรายการ
/// </summary>
/// <remarks>
/// ไม่มี Endpoint แก้ไขหรือลบโดยตั้งใจ (มีเฉพาะสิทธิ์ View และ Issue) เพราะถือเป็นเอกสารหลักฐาน
/// การชำระเงินที่ต้องคงที่ตลอดไป ออกได้ครั้งเดียวต่อรายการชำระเงินหนึ่งรายการ
/// จำนวนเงินคัดลอกมาจาก Payment ตอนออกใบเสร็จ ไม่อ้างอิงแบบ Real-time
/// เพื่อให้เอกสารที่ออกไปแล้วไม่เปลี่ยนแปลงย้อนหลังแม้ข้อมูลต้นทางจะถูกแก้ไข
/// </remarks>
public class Receipt : TenantEntity
{
    /// <summary>รายการชำระเงินที่ออกใบเสร็จนี้ให้</summary>
    public Guid PaymentId { get; set; }

    /// <summary>เลขที่ใบเสร็จ ไม่ซ้ำกันภายในองค์กรเดียวกัน</summary>
    public required string ReceiptNumber { get; set; }

    /// <summary>จำนวนเงินตามใบเสร็จ คัดลอกมาจาก Payment ตอนออกใบเสร็จ</summary>
    public decimal Amount { get; set; }

    /// <summary>เวลาที่ออกใบเสร็จ</summary>
    public DateTimeOffset IssuedAt { get; set; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
