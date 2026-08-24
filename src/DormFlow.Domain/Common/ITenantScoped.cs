namespace DormFlow.Domain.Common;

/// <summary>
/// Entity ที่ผูกกับองค์กร (Tenant) ทุก Query ต้องกรองด้วย <see cref="OrganizationId"/>
/// และกรองด้วย <see cref="DormitoryId"/> เพิ่มเมื่อข้อมูลผูกกับหอพักใดหอพักหนึ่ง
/// </summary>
public interface ITenantScoped
{
    /// <summary>องค์กรเจ้าของข้อมูล</summary>
    Guid OrganizationId { get; set; }

    /// <summary>หอพักเจ้าของข้อมูล เป็น null เมื่อข้อมูลอยู่ระดับองค์กร</summary>
    Guid? DormitoryId { get; set; }
}
