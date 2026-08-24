using DormFlow.Domain.Common;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ค่าตั้งหนึ่งรายการขององค์กร เก็บเป็นคู่ Key/Value อิสระ
/// </summary>
/// <remarks>
/// สืบทอดจาก <see cref="BaseEntity"/> ไม่ใช่ <see cref="TenantEntity"/> เพราะค่าตั้งเป็นระดับองค์กรเท่านั้น
/// ไม่ผูกกับหอพักใดหอพักหนึ่ง เช่นเดียวกับ <see cref="Tenant"/> — Key ไม่ซ้ำกันภายในองค์กรเดียวกัน
/// (unique index บน OrganizationId+Key) เก็บเป็น Key/Value อิสระแทนที่จะมีคอลัมน์ตายตัวทีละค่าตั้ง
/// เพราะเอกสารต้นฉบับไม่ได้ระบุรายการค่าตั้งที่ต้องมีไว้เลย
/// </remarks>
public class Setting : BaseEntity
{
    /// <summary>องค์กรเจ้าของค่าตั้งนี้</summary>
    public Guid OrganizationId { get; set; }

    /// <summary>ชื่อคีย์ของค่าตั้ง</summary>
    public required string Key { get; set; }

    /// <summary>ค่าของค่าตั้ง เก็บเป็นข้อความอิสระ (อาจเป็น JSON ก็ได้)</summary>
    public required string Value { get; set; }
}
