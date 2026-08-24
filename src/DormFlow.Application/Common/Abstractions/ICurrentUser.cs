namespace DormFlow.Application.Common.Abstractions;

/// <summary>
/// ข้อมูลผู้ใช้ของคำขอปัจจุบัน อ่านจาก Claim ของ JWT
/// ใช้เป็นแหล่งความจริงเดียวสำหรับการกรองข้อมูลตามองค์กรและหอพัก
/// </summary>
public interface ICurrentUser
{
    /// <summary>รหัสผู้ใช้ เป็น null เมื่อคำขอยังไม่ผ่านการยืนยันตัวตน</summary>
    Guid? UserId { get; }

    /// <summary>องค์กรที่ผู้ใช้สังกัด เป็น null สำหรับ SuperAdmin ที่ไม่ผูกกับองค์กรใด</summary>
    Guid? OrganizationId { get; }

    /// <summary>ผู้ใช้ผ่านการยืนยันตัวตนแล้วหรือไม่</summary>
    bool IsAuthenticated { get; }

    /// <summary>ผู้ใช้เป็น SuperAdmin ที่เข้าถึงข้อมูลข้ามองค์กรได้หรือไม่</summary>
    bool IsSuperAdmin { get; }

    /// <summary>รายชื่อบทบาทของผู้ใช้</summary>
    IReadOnlyCollection<string> Roles { get; }

    /// <summary>รายการหอพักที่ผู้ใช้ถูกมอบหมายให้ดูแล</summary>
    IReadOnlyCollection<Guid> DormitoryIds { get; }

    /// <summary>รายการรหัสสิทธิ์ที่ผู้ใช้มี เช่น rooms.create</summary>
    IReadOnlyCollection<string> Permissions { get; }

    /// <summary>หมายเลข IP ต้นทางของคำขอ ใช้บันทึกใน Audit Log และ Refresh Token</summary>
    string? IpAddress { get; }

    /// <summary>User Agent ของคำขอ ใช้บันทึกใน Audit Log และ Refresh Token</summary>
    string? UserAgent { get; }

    /// <summary>เส้นทางของคำขอปัจจุบัน ใช้บันทึกใน Audit Log</summary>
    string? RequestPath { get; }

    /// <summary>ตรวจว่าผู้ใช้มีสิทธิ์ที่ระบุหรือไม่ SuperAdmin ถือว่ามีทุกสิทธิ์เสมอ</summary>
    /// <param name="permissionCode">รหัสสิทธิ์ เช่น rooms.change-status</param>
    bool HasPermission(string permissionCode);

    /// <summary>ตรวจว่าผู้ใช้เข้าถึงข้อมูลของหอพักที่ระบุได้หรือไม่</summary>
    /// <param name="dormitoryId">รหัสหอพัก</param>
    bool CanAccessDormitory(Guid dormitoryId);
}
