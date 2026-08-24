namespace DormFlow.Contracts.Settings;

/// <summary>ข้อมูลค่าตั้งหนึ่งรายการ</summary>
public class SettingResponse
{
    /// <summary>รหัสอ้างอิงของค่าตั้ง</summary>
    public Guid Id { get; init; }

    /// <summary>องค์กรเจ้าของค่าตั้งนี้</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>ชื่อคีย์ของค่าตั้ง</summary>
    public string Key { get; init; } = string.Empty;

    /// <summary>ค่าของค่าตั้ง</summary>
    public string Value { get; init; } = string.Empty;

    /// <summary>เวลาที่สร้างข้อมูล</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>เวลาที่แก้ไขล่าสุด</summary>
    public DateTimeOffset? UpdatedAt { get; init; }
}
