using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>ห้องพักหนึ่งห้อง</summary>
/// <remarks>
/// เก็บ <see cref="MonthlyRent"/>, <see cref="DepositAmount"/> และ <see cref="MaximumOccupants"/>
/// แยกจากค่าของ <see cref="RoomType"/> เพื่อให้ปรับราคาห้องเป็นรายห้องได้
/// ค่าเริ่มต้นตอนสร้างห้องใหม่ควรคัดลอกมาจากประเภทห้องที่เลือก
/// </remarks>
public class Room : TenantEntity
{
    /// <summary>อาคารที่ห้องนี้สังกัด</summary>
    public Guid BuildingId { get; set; }

    /// <summary>ชั้นที่ห้องนี้สังกัด</summary>
    public Guid FloorId { get; set; }

    /// <summary>ประเภทของห้อง</summary>
    public Guid RoomTypeId { get; set; }

    /// <summary>เลขห้อง ไม่ซ้ำกันภายในอาคารเดียวกัน</summary>
    public required string RoomNumber { get; set; }

    /// <summary>ค่าเช่ารายเดือนของห้องนี้</summary>
    public decimal MonthlyRent { get; set; }

    /// <summary>ค่ามัดจำของห้องนี้</summary>
    public decimal DepositAmount { get; set; }

    /// <summary>จำนวนผู้เข้าพักสูงสุดของห้องนี้</summary>
    public int MaximumOccupants { get; set; } = 1;

    /// <summary>สถานะของห้อง</summary>
    public RoomStatus Status { get; set; } = RoomStatus.Available;

    /// <summary>คำอธิบายเพิ่มเติม</summary>
    public string? Description { get; set; }
}
