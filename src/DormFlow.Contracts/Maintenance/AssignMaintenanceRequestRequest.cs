namespace DormFlow.Contracts.Maintenance;

/// <summary>คำขอมอบหมายช่างซ่อมบำรุงให้กับงานซ่อม</summary>
public class AssignMaintenanceRequestRequest
{
    /// <summary>ช่างซ่อมบำรุงที่ต้องการมอบหมาย ต้องมีบทบาท Technician และได้รับมอบหมายดูแลหอพักนี้</summary>
    public Guid TechnicianId { get; set; }
}
