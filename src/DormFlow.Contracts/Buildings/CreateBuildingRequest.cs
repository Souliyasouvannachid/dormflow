namespace DormFlow.Contracts.Buildings;

/// <summary>คำขอสร้างอาคารใหม่</summary>
public class CreateBuildingRequest
{
    /// <summary>หอพักที่อาคารนี้จะสังกัด</summary>
    public Guid DormitoryId { get; set; }

    /// <summary>รหัสอาคาร ไม่ซ้ำกันภายในหอพักเดียวกัน</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>ชื่ออาคาร</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>คำอธิบายเพิ่มเติม</summary>
    public string? Description { get; set; }

    /// <summary>จำนวนชั้นทั้งหมดของอาคาร</summary>
    public int TotalFloors { get; set; } = 1;
}
