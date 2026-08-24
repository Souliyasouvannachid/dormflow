namespace DormFlow.Contracts.Maintenance;

/// <summary>คำขอแจ้งซ่อมบำรุง</summary>
public class CreateMaintenanceRequestRequest
{
    /// <summary>ห้องที่แจ้งซ่อม</summary>
    public Guid RoomId { get; set; }

    /// <summary>หัวข้อของงานซ่อม</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>รายละเอียดเพิ่มเติมของปัญหา</summary>
    public string? Description { get; set; }

    /// <summary>ระดับความสำคัญ ("Low", "Medium", "High", "Urgent")</summary>
    public string Priority { get; set; } = "Medium";
}
