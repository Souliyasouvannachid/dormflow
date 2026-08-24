namespace DormFlow.Contracts.Floors;

/// <summary>คำขอแก้ไขข้อมูลชั้น</summary>
public class UpdateFloorRequest
{
    /// <summary>ลำดับชั้น ไม่ซ้ำกันภายในอาคารเดียวกัน</summary>
    public int FloorNumber { get; set; }

    /// <summary>ชื่อชั้น สำหรับแสดงผล</summary>
    public string? Name { get; set; }

    /// <summary>คำอธิบายเพิ่มเติม</summary>
    public string? Description { get; set; }
}
