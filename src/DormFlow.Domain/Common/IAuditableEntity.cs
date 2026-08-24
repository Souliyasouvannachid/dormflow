namespace DormFlow.Domain.Common;

/// <summary>
/// Entity ที่ต้องบันทึกร่องรอยการสร้างและการแก้ไข
/// ค่าเหล่านี้ถูกเติมอัตโนมัติโดย Interceptor ของ EF Core ไม่ควรกำหนดเองจาก Use Case
/// </summary>
public interface IAuditableEntity
{
    /// <summary>เวลาที่สร้างข้อมูล (timestamptz)</summary>
    DateTimeOffset CreatedAt { get; set; }

    /// <summary>ผู้สร้างข้อมูล เป็น null เมื่อสร้างโดยระบบ เช่น ตอน Seed</summary>
    Guid? CreatedBy { get; set; }

    /// <summary>เวลาที่แก้ไขข้อมูลครั้งล่าสุด</summary>
    DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>ผู้แก้ไขข้อมูลครั้งล่าสุด</summary>
    Guid? UpdatedBy { get; set; }
}
