namespace DormFlow.Contracts.Roles;

/// <summary>ข้อมูลบทบาทหนึ่งบทบาท</summary>
public class RoleResponse
{
    /// <summary>รหัสอ้างอิงของบทบาท</summary>
    public Guid Id { get; init; }

    /// <summary>ชื่อบทบาท</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>คำอธิบายบทบาท</summary>
    public string? Description { get; init; }

    /// <summary>
    /// เป็นบทบาทมาตรฐานของระบบหรือไม่
    /// บทบาทมาตรฐานเปลี่ยนชื่อและลบไม่ได้ แต่ปรับสิทธิ์ได้ตามปกติ
    /// </summary>
    public bool IsSystemRole { get; init; }

    /// <summary>รหัสสิทธิ์ทั้งหมดที่บทบาทนี้มี</summary>
    public IReadOnlyList<string> Permissions { get; init; } = [];
}
