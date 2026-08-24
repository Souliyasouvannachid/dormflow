namespace DormFlow.Contracts.Permissions;

/// <summary>ข้อมูลสิทธิ์การใช้งานหนึ่งรายการ</summary>
public class PermissionResponse
{
    /// <summary>รหัสสิทธิ์ ใช้อ้างอิงตอนกำหนดสิทธิ์ให้บทบาท</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>ชื่อสิทธิ์สำหรับแสดงผล</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>คำอธิบายเพิ่มเติม</summary>
    public string? Description { get; init; }

    /// <summary>โมดูลที่สิทธิ์นี้สังกัด</summary>
    public string Module { get; init; } = string.Empty;
}
