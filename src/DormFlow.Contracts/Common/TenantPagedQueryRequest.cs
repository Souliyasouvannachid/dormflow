namespace DormFlow.Contracts.Common;

/// <summary>
/// พารามิเตอร์ของ List Endpoint ที่กรองข้อมูลตามองค์กรและหอพักได้
/// </summary>
/// <remarks>
/// ค่าที่ระบุมาเป็นเพียงตัวกรองเพิ่มเติมเท่านั้น
/// ขอบเขตข้อมูลที่ผู้ใช้เห็นได้จริงถูกบังคับจากสิทธิ์ของผู้ใช้ที่ฝั่งเซิร์ฟเวอร์เสมอ
/// </remarks>
public class TenantPagedQueryRequest : PagedQueryRequest
{
    /// <summary>กรองเฉพาะข้อมูลขององค์กรที่ระบุ</summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>กรองเฉพาะข้อมูลของหอพักที่ระบุ</summary>
    public Guid? DormitoryId { get; set; }
}
