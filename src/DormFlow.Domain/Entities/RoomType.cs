using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>ประเภทห้องพักหนึ่งแบบภายในหอพัก เช่น ห้องแอร์ ห้องพัดลม</summary>
public class RoomType : TenantEntity
{
    /// <summary>รหัสประเภทห้อง ไม่ซ้ำกันภายในหอพักเดียวกัน</summary>
    public required string Code { get; set; }

    /// <summary>ชื่อประเภทห้อง</summary>
    public required string Name { get; set; }

    /// <summary>คำอธิบายเพิ่มเติม</summary>
    public string? Description { get; set; }

    /// <summary>ค่าเช่ารายเดือน</summary>
    public decimal MonthlyRent { get; set; }

    /// <summary>ค่าเช่ารายวัน เป็น null เมื่อประเภทห้องนี้ไม่รองรับการเช่ารายวัน</summary>
    public decimal? DailyRent { get; set; }

    /// <summary>ค่ามัดจำ</summary>
    public decimal DepositAmount { get; set; }

    /// <summary>จำนวนผู้เข้าพักสูงสุด</summary>
    public int MaximumOccupants { get; set; } = 1;

    /// <summary>สถานะของประเภทห้อง</summary>
    public RoomTypeStatus Status { get; set; } = RoomTypeStatus.Active;
}
