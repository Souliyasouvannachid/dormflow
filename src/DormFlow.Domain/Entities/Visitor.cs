using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// บันทึกการเข้า-ออกของผู้มาเยือนหนึ่งครั้ง
/// </summary>
/// <remarks>
/// ผูกกับห้องเสมอ (เพื่อขอบเขตองค์กร/หอพักและระบุว่ามาเยี่ยมห้องไหน) ส่วนผู้เช่าเป็นทางเลือก
/// เช่นเดียวกับ <see cref="Parcel"/> เพราะห้องอาจมีผู้เช่าร่วมหลายคน
/// </remarks>
public class Visitor : TenantEntity
{
    /// <summary>ห้องที่ผู้มาเยือนมาเยี่ยม</summary>
    public Guid RoomId { get; set; }

    /// <summary>ผู้เช่าที่ผู้มาเยือนมาเยี่ยม เป็น null เมื่อไม่ระบุ</summary>
    public Guid? TenantId { get; set; }

    /// <summary>ชื่อผู้มาเยือน</summary>
    public required string VisitorName { get; set; }

    /// <summary>เบอร์โทรศัพท์ผู้มาเยือน</summary>
    public string? VisitorPhone { get; set; }

    /// <summary>เลขบัตรประจำตัวประชาชนหรือหนังสือเดินทางของผู้มาเยือน สำหรับบันทึกความปลอดภัย</summary>
    public string? IdNumber { get; set; }

    /// <summary>วัตถุประสงค์การมาเยือน</summary>
    public string? Purpose { get; set; }

    /// <summary>เวลาที่เข้าหอพัก</summary>
    public DateTimeOffset CheckInAt { get; set; }

    /// <summary>สถานะของผู้มาเยือน</summary>
    public VisitorStatus Status { get; set; } = VisitorStatus.CheckedIn;

    /// <summary>เวลาที่ออกจากหอพัก เป็น null เมื่อยังไม่ออก</summary>
    public DateTimeOffset? CheckOutAt { get; set; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
