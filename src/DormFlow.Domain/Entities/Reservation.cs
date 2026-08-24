using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// การจองห้องพักหนึ่งครั้งก่อนทำสัญญาเช่าจริง
/// </summary>
/// <remarks>
/// การจองทำให้ห้องเปลี่ยนสถานะเป็น <see cref="RoomStatus.Reserved"/> ทันที
/// เพื่อกันไม่ให้ถูกจองซ้ำระหว่างที่ผู้เช่ากำลังตัดสินใจหรือเตรียมเอกสารทำสัญญา
/// </remarks>
public class Reservation : TenantEntity
{
    /// <summary>ห้องที่ถูกจอง</summary>
    public Guid RoomId { get; set; }

    /// <summary>ผู้เช่าที่จอง</summary>
    public Guid TenantId { get; set; }

    /// <summary>วันที่ทำการจอง</summary>
    public DateTimeOffset ReservationDate { get; set; }

    /// <summary>วันที่คาดว่าจะเข้าพัก</summary>
    public DateOnly MoveInDate { get; set; }

    /// <summary>เวลาที่การกันห้องจะหมดอายุ ถ้ายังไม่ทำสัญญาก่อนเวลานี้ห้องจะถูกปล่อยว่างคืน</summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>ค่าจองห้อง</summary>
    public decimal ReservationFee { get; set; }

    /// <summary>สถานะของการจอง</summary>
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
