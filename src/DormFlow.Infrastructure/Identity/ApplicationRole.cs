using Microsoft.AspNetCore.Identity;

namespace DormFlow.Infrastructure.Identity;

/// <summary>บทบาทผู้ใช้งาน ต่อยอดจาก <see cref="IdentityRole{TKey}"/></summary>
public class ApplicationRole : IdentityRole<Guid>
{
    /// <summary>สร้างบทบาทเปล่า ใช้โดย EF Core ตอนสร้างอ็อบเจกต์จากฐานข้อมูล</summary>
    public ApplicationRole()
    {
    }

    /// <summary>สร้างบทบาทจากชื่อ</summary>
    /// <param name="roleName">ชื่อบทบาท</param>
    public ApplicationRole(string roleName)
        : base(roleName)
    {
    }

    /// <summary>คำอธิบายบทบาท</summary>
    public string? Description { get; set; }

    /// <summary>
    /// เป็นบทบาทมาตรฐานของระบบหรือไม่
    /// บทบาทที่เป็นของระบบจะลบไม่ได้ เพราะมีการอ้างถึงในโค้ดและในข้อมูลเริ่มต้น
    /// </summary>
    public bool IsSystemRole { get; set; }
}
