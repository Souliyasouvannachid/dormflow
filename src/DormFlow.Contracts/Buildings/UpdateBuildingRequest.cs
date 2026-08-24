namespace DormFlow.Contracts.Buildings;

/// <summary>คำขอแก้ไขข้อมูลอาคาร</summary>
/// <remarks>ไม่มีฟิลด์ DormitoryId เพราะการย้ายอาคารข้ามหอพักไม่ใช่การแก้ไขข้อมูลปกติ</remarks>
public class UpdateBuildingRequest
{
    /// <summary>รหัสอาคาร ไม่ซ้ำกันภายในหอพักเดียวกัน</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>ชื่ออาคาร</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>คำอธิบายเพิ่มเติม</summary>
    public string? Description { get; set; }

    /// <summary>จำนวนชั้นทั้งหมดของอาคาร</summary>
    public int TotalFloors { get; set; } = 1;
}
