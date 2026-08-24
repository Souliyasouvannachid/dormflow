namespace DormFlow.Contracts.Floors;

/// <summary>คำขอสร้างชั้นใหม่</summary>
public class CreateFloorRequest
{
    /// <summary>อาคารที่ชั้นนี้จะสังกัด</summary>
    public Guid BuildingId { get; set; }

    /// <summary>ลำดับชั้น ไม่ซ้ำกันภายในอาคารเดียวกัน</summary>
    public int FloorNumber { get; set; }

    /// <summary>ชื่อชั้น สำหรับแสดงผล เช่น "ชั้น G"</summary>
    public string? Name { get; set; }

    /// <summary>คำอธิบายเพิ่มเติม</summary>
    public string? Description { get; set; }
}
