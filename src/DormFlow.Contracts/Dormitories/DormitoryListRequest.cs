using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Dormitories;

/// <summary>พารามิเตอร์การค้นหารายการหอพัก</summary>
public class DormitoryListRequest : PagedQueryRequest
{
    /// <summary>
    /// กรองเฉพาะหอพักขององค์กรที่ระบุ มีผลเฉพาะเมื่อผู้เรียกเป็น SuperAdmin
    /// ผู้ใช้อื่นเห็นได้เฉพาะหอพักในองค์กรของตนเองเสมอไม่ว่าจะระบุค่านี้หรือไม่
    /// </summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>กรองเฉพาะหอพักที่มีสถานะตรงกับที่ระบุ</summary>
    public string? Status { get; set; }
}
