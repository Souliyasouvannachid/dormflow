using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Settings;

/// <summary>พารามิเตอร์การค้นหารายการค่าตั้ง</summary>
public class SettingListRequest : PagedQueryRequest
{
    /// <summary>องค์กรเป้าหมาย จำเป็นเฉพาะเมื่อผู้เรียกเป็น SuperAdmin</summary>
    public Guid? OrganizationId { get; set; }
}
