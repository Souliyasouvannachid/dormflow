namespace DormFlow.Domain.Common;

/// <summary>
/// คลาสฐานของ Entity ฝั่งธุรกิจทั้งหมดที่ต้องแยกข้อมูลตามองค์กรและหอพัก
/// ทุก Query ที่อ่าน Entity ชนิดนี้ต้องกรองด้วย OrganizationId เสมอ
/// </summary>
public abstract class TenantEntity : BaseEntity, ITenantScoped
{
    /// <inheritdoc />
    public Guid OrganizationId { get; set; }

    /// <inheritdoc />
    public Guid? DormitoryId { get; set; }
}
