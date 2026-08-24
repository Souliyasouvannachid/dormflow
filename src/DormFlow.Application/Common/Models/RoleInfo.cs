namespace DormFlow.Application.Common.Models;

/// <summary>
/// ข้อมูลบทบาทหนึ่งบทบาท ใช้แทน Entity ของ ASP.NET Core Identity ในชั้น Application
/// </summary>
/// <param name="Id">รหัสอ้างอิงของบทบาท</param>
/// <param name="Name">ชื่อบทบาท</param>
/// <param name="Description">คำอธิบายบทบาท</param>
/// <param name="IsSystemRole">เป็นบทบาทมาตรฐานของระบบหรือไม่</param>
/// <param name="Permissions">รหัสสิทธิ์ทั้งหมดที่บทบาทนี้มี</param>
public sealed record RoleInfo(
    Guid Id,
    string Name,
    string? Description,
    bool IsSystemRole,
    IReadOnlyList<string> Permissions);
