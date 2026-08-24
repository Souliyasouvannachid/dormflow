using DormFlow.Application.Common.Models;
using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Users;

/// <summary>ตัวกรองสำหรับดึงรายการผู้ใช้แบบแบ่งหน้า</summary>
/// <param name="OrganizationId">กรองเฉพาะผู้ใช้ขององค์กรที่ระบุ เป็น null เมื่อไม่จำกัด</param>
/// <param name="DormitoryId">กรองเฉพาะผู้ใช้ที่ได้รับมอบหมายให้ดูแลหอพักที่ระบุ เป็น null เมื่อไม่จำกัด</param>
/// <param name="Status">กรองเฉพาะผู้ใช้ที่มีสถานะตรงกัน เป็น null เมื่อไม่จำกัด</param>
/// <param name="RestrictToDormitoryIds">
/// จำกัดผลลัพธ์ให้เห็นเฉพาะผู้ใช้ที่ได้รับมอบหมายให้ดูแลหอพักในรายการนี้อย่างน้อยหนึ่งแห่ง
/// เป็น null เมื่อผู้เรียกเห็นได้ทุกผู้ใช้ในองค์กรของตน (SuperAdmin และ Owner)
/// </param>
public sealed record UserListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    UserStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ผลลัพธ์ของการสร้างบัญชีผู้ใช้ใหม่</summary>
/// <param name="Succeeded">สำเร็จหรือไม่</param>
/// <param name="UserId">รหัสผู้ใช้ที่สร้างสำเร็จ มีค่าเฉพาะเมื่อสำเร็จ</param>
/// <param name="Errors">ข้อความอธิบายสาเหตุที่ไม่สำเร็จ</param>
public sealed record UserCreationResult(bool Succeeded, Guid? UserId, IReadOnlyList<string> Errors)
{
    /// <summary>สร้างผลลัพธ์ที่สำเร็จ</summary>
    public static UserCreationResult Success(Guid userId) => new(true, userId, []);

    /// <summary>สร้างผลลัพธ์ที่ไม่สำเร็จ</summary>
    public static UserCreationResult Failed(params string[] errors) => new(false, null, errors);
}

/// <summary>
/// เข้าถึงและจัดการบัญชีผู้ใช้ในฐานข้อมูล สำหรับหน้าจอดูแลผู้ใช้งานของผู้ดูแลระบบ
/// </summary>
/// <remarks>
/// แยกจาก <see cref="Common.Abstractions.IUserAccountService"/> เพราะเป็นคนละความรับผิดชอบ
/// ตัวนั้นดูแลเรื่องยืนยันตัวตนและรหัสผ่านของผู้ใช้เจ้าของบัญชีเอง
/// ส่วนตัวนี้ดูแลเรื่องการบริหารจัดการบัญชีผู้ใช้อื่นโดยผู้ดูแลระบบ
/// </remarks>
public interface IUserAdministrationRepository
{
    /// <summary>ตรวจว่ามีบัญชีที่ใช้อีเมลนี้อยู่แล้วหรือไม่</summary>
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลผู้ใช้จากรหัสอ้างอิง</summary>
    Task<UserSecurityProfile?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการผู้ใช้แบบแบ่งหน้าตามตัวกรองที่ระบุ</summary>
    Task<PagedResult<UserSecurityProfile>> GetPagedAsync(
        UserListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>สร้างบัญชีผู้ใช้ใหม่</summary>
    Task<UserCreationResult> CreateAsync(
        Guid? organizationId,
        string email,
        string password,
        string firstName,
        string lastName,
        string? phone,
        CancellationToken cancellationToken = default);

    /// <summary>แก้ไขข้อมูลโปรไฟล์ของผู้ใช้ คืนค่า false เมื่อไม่พบผู้ใช้</summary>
    Task<bool> UpdateProfileAsync(
        Guid userId,
        string firstName,
        string lastName,
        string? phone,
        string? profileImage,
        CancellationToken cancellationToken = default);

    /// <summary>เปลี่ยนสถานะบัญชีผู้ใช้ คืนค่า false เมื่อไม่พบผู้ใช้</summary>
    Task<bool> ChangeStatusAsync(
        Guid userId,
        UserStatus status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// กำหนดบทบาทให้ผู้ใช้ แทนที่บทบาทเดิมทั้งหมดด้วยรายการที่ระบุ
    /// คืนค่า false เมื่อไม่พบผู้ใช้ หรือพบชื่อบทบาทที่ไม่มีอยู่จริงในระบบ
    /// </summary>
    Task<bool> SetRolesAsync(
        Guid userId,
        IReadOnlyList<string> roleNames,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// มอบหมายให้ผู้ใช้ดูแลหอพักหนึ่งแห่ง คืนค่า false เมื่อไม่พบผู้ใช้หรือไม่พบหอพัก
    /// เมื่อตั้งเป็นหอพักหลัก หอพักหลักเดิมของผู้ใช้รายนี้ (ถ้ามี) จะถูกยกเลิกสถานะหอพักหลักให้อัตโนมัติ
    /// </summary>
    Task<bool> AssignDormitoryAsync(
        Guid userId,
        Guid dormitoryId,
        bool isPrimary,
        Guid? assignedBy,
        DateTimeOffset assignedAt,
        CancellationToken cancellationToken = default);

    /// <summary>ถอนการมอบหมายหอพักออกจากผู้ใช้ คืนค่า false เมื่อไม่พบการมอบหมายนั้น</summary>
    Task<bool> RemoveDormitoryAsync(
        Guid userId,
        Guid dormitoryId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ตรวจว่าผู้ใช้ที่ระบุสังกัดองค์กรนี้ มีบทบาทที่กำหนด และได้รับมอบหมายให้ดูแลหอพักที่ระบุหรือไม่
    /// ใช้ตรวจสอบก่อนมอบหมายงาน เช่น การมอบหมายช่างซ่อมบำรุงให้ดูแลงานซ่อมในหอพักหนึ่ง ๆ
    /// </summary>
    Task<bool> HasRoleForDormitoryAsync(
        Guid userId,
        string roleName,
        Guid organizationId,
        Guid dormitoryId,
        CancellationToken cancellationToken = default);
}
