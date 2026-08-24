using Microsoft.AspNetCore.Authorization;

namespace DormFlow.Api.Authorization;

/// <summary>
/// ประดับ Endpoint ที่ต้องการสิทธิ์เฉพาะ
/// </summary>
/// <remarks>
/// ใช้แทน [Authorize(Policy = "...")] ตรง ๆ เพื่อให้อ่านโค้ดของ Controller เข้าใจง่ายกว่า
/// และเพื่อให้ชื่อ Policy ที่แท้จริงถูกสร้างจากรหัสสิทธิ์แบบอัตโนมัติในจุดเดียว
/// </remarks>
public sealed class RequirePermissionAttribute : AuthorizeAttribute
{
    /// <summary>คำนำหน้าของทุก Policy ที่สร้างจากสิทธิ์ ใช้แยกจาก Policy ประเภทอื่น</summary>
    public const string PolicyPrefix = "Permission:";

    /// <summary>สร้าง Attribute จากรหัสสิทธิ์ที่ต้องการ</summary>
    /// <param name="permissionCode">รหัสสิทธิ์ เช่น rooms.create</param>
    public RequirePermissionAttribute(string permissionCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permissionCode);
        Policy = PolicyPrefix + permissionCode;
    }
}
