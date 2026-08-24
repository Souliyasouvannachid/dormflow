namespace DormFlow.Contracts.Roles;

/// <summary>คำขอสร้างบทบาทใหม่</summary>
public class CreateRoleRequest
{
    /// <summary>ชื่อบทบาท ไม่ซ้ำกันทั้งระบบ</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>คำอธิบายบทบาท</summary>
    public string? Description { get; set; }
}
