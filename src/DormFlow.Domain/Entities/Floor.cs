using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>ชั้นหนึ่งของอาคาร</summary>
public class Floor : TenantEntity
{
    /// <summary>อาคารที่ชั้นนี้สังกัด</summary>
    public Guid BuildingId { get; set; }

    /// <summary>ลำดับชั้น</summary>
    public int FloorNumber { get; set; }

    /// <summary>ชื่อชั้น สำหรับแสดงผล เช่น "ชั้น G"</summary>
    public string? Name { get; set; }

    /// <summary>คำอธิบายเพิ่มเติม</summary>
    public string? Description { get; set; }

    /// <summary>สถานะของชั้น</summary>
    public FloorStatus Status { get; set; } = FloorStatus.Active;
}
