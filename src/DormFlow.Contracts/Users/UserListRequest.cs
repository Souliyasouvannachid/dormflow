using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Users;

/// <summary>พารามิเตอร์การค้นหารายการผู้ใช้</summary>
public class UserListRequest : PagedQueryRequest
{
    /// <summary>
    /// กรองเฉพาะผู้ใช้ขององค์กรที่ระบุ มีผลเฉพาะเมื่อผู้เรียกเป็น SuperAdmin
    /// ผู้ใช้อื่นเห็นได้เฉพาะผู้ใช้ในองค์กรของตนเองเสมอไม่ว่าจะระบุค่านี้หรือไม่
    /// </summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>กรองเฉพาะผู้ใช้ที่ได้รับมอบหมายให้ดูแลหอพักที่ระบุ</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>กรองเฉพาะผู้ใช้ที่มีสถานะตรงกับที่ระบุ</summary>
    public string? Status { get; set; }
}
