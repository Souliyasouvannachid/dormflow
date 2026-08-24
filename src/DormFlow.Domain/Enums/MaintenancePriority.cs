namespace DormFlow.Domain.Enums;

/// <summary>ระดับความสำคัญของงานซ่อมบำรุง</summary>
public enum MaintenancePriority
{
    /// <summary>ต่ำ</summary>
    Low = 1,

    /// <summary>ปานกลาง</summary>
    Medium = 2,

    /// <summary>สูง</summary>
    High = 3,

    /// <summary>เร่งด่วน</summary>
    Urgent = 4,
}
