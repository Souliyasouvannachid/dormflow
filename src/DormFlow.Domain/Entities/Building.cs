using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>อาคารหนึ่งหลังภายในหอพัก</summary>
public class Building : TenantEntity
{
    /// <summary>รหัสอาคาร ไม่ซ้ำกันภายในหอพักเดียวกัน</summary>
    public required string Code { get; set; }

    /// <summary>ชื่ออาคาร</summary>
    public required string Name { get; set; }

    /// <summary>คำอธิบายเพิ่มเติม</summary>
    public string? Description { get; set; }

    /// <summary>จำนวนชั้นทั้งหมดของอาคาร</summary>
    public int TotalFloors { get; set; } = 1;

    /// <summary>สถานะของอาคาร</summary>
    public BuildingStatus Status { get; set; } = BuildingStatus.Active;
}
