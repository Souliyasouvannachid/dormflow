namespace DormFlow.Domain.Enums;

/// <summary>สถานะของผู้เช่า</summary>
public enum TenantStatus
{
    /// <summary>ใช้งานได้ตามปกติ</summary>
    Active = 1,

    /// <summary>ปิดใช้งานชั่วคราว เช่น ไม่มีสัญญาที่ยังใช้งานอยู่</summary>
    Inactive = 2,

    /// <summary>ขึ้นบัญชีดำ ห้ามทำสัญญาเช่าใหม่</summary>
    Blacklisted = 3,
}
