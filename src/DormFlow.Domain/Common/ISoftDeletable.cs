namespace DormFlow.Domain.Common;

/// <summary>
/// Entity ที่ใช้ Soft Delete ระบบจะไม่ลบแถวจริง แต่ตั้งค่า <see cref="IsDeleted"/> เป็น true
/// และถูกกรองออกอัตโนมัติด้วย Global Query Filter
/// </summary>
public interface ISoftDeletable
{
    /// <summary>ถูกลบแล้วหรือไม่</summary>
    bool IsDeleted { get; set; }

    /// <summary>เวลาที่ถูกลบ</summary>
    DateTimeOffset? DeletedAt { get; set; }

    /// <summary>ผู้ที่ลบข้อมูล</summary>
    Guid? DeletedBy { get; set; }
}
