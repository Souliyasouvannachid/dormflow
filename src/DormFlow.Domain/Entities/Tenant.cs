using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ผู้เช่าคนหนึ่ง
/// </summary>
/// <remarks>
/// สืบทอดจาก <see cref="BaseEntity"/> ไม่ใช่ <see cref="TenantEntity"/>
/// เพราะผู้เช่าไม่ได้ผูกกับหอพักใดหอพักหนึ่งถาวร ในช่วงเวลาต่างกันอาจมีสัญญาเช่าคนละหอพักในองค์กรเดียวกันได้
/// ความสัมพันธ์กับหอพักที่กำลังพักอยู่ดูได้จาก <see cref="Contract"/> ที่ยัง Active เท่านั้น
/// </remarks>
public class Tenant : BaseEntity
{
    /// <summary>องค์กรที่ผู้เช่ารายนี้สังกัด</summary>
    public Guid OrganizationId { get; set; }

    /// <summary>ชื่อ</summary>
    public required string FirstName { get; set; }

    /// <summary>นามสกุล</summary>
    public required string LastName { get; set; }

    /// <summary>อีเมล</summary>
    public string? Email { get; set; }

    /// <summary>เบอร์โทรศัพท์</summary>
    public string? Phone { get; set; }

    /// <summary>เลขบัตรประจำตัวประชาชนหรือหนังสือเดินทาง</summary>
    public string? NationalId { get; set; }

    /// <summary>วันเดือนปีเกิด</summary>
    public DateOnly? DateOfBirth { get; set; }

    /// <summary>ที่อยู่ตามทะเบียนบ้าน</summary>
    public string? Address { get; set; }

    /// <summary>ชื่อผู้ติดต่อฉุกเฉิน</summary>
    public string? EmergencyContactName { get; set; }

    /// <summary>เบอร์โทรศัพท์ผู้ติดต่อฉุกเฉิน</summary>
    public string? EmergencyContactPhone { get; set; }

    /// <summary>สถานะของผู้เช่า</summary>
    public TenantStatus Status { get; set; } = TenantStatus.Active;

    /// <summary>ชื่อเต็มสำหรับแสดงผล</summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
}
