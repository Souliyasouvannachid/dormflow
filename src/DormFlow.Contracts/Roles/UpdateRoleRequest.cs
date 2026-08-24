namespace DormFlow.Contracts.Roles;

/// <summary>คำขอแก้ไขข้อมูลบทบาท</summary>
/// <remarks>ระบบไม่ยอมเปลี่ยนชื่อของบทบาทมาตรฐาน แม้จะระบุค่าอื่นมาในฟิลด์ Name ก็ตาม</remarks>
public class UpdateRoleRequest
{
    /// <summary>ชื่อบทบาท ไม่ซ้ำกันทั้งระบบ</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>คำอธิบายบทบาท</summary>
    public string? Description { get; set; }
}
