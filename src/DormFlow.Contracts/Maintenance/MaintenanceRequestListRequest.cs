using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Maintenance;

/// <summary>พารามิเตอร์การค้นหารายการงานแจ้งซ่อมบำรุง</summary>
public class MaintenanceRequestListRequest : PagedQueryRequest
{
    /// <summary>กรองเฉพาะรายการของหอพักที่ระบุ</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>กรองเฉพาะรายการของห้องที่ระบุ</summary>
    public Guid? RoomId { get; set; }

    /// <summary>กรองเฉพาะรายการที่มีสถานะตรงกับที่ระบุ</summary>
    public string? Status { get; set; }

    /// <summary>กรองเฉพาะรายการที่มีระดับความสำคัญตรงกับที่ระบุ</summary>
    public string? Priority { get; set; }

    /// <summary>กรองเฉพาะรายการที่มอบหมายให้ช่างที่ระบุ</summary>
    public Guid? AssignedTechnicianId { get; set; }
}
