namespace DormFlow.Application.Common.Abstractions;

/// <summary>
/// บทบาทผู้ใช้งานทั้งหมดของระบบ
/// </summary>
public static class DormFlowRoles
{
    /// <summary>ผู้ดูแลระบบสูงสุด เข้าถึงข้อมูลได้ทุกองค์กร</summary>
    public const string SuperAdmin = "SuperAdmin";

    /// <summary>เจ้าของกิจการ ดูแลได้ทุกหอพักในองค์กรของตน</summary>
    public const string Owner = "Owner";

    /// <summary>ผู้จัดการหอพัก ดูแลเฉพาะหอพักที่ได้รับมอบหมาย</summary>
    public const string DormManager = "DormManager";

    /// <summary>ฝ่ายบัญชี</summary>
    public const string Accountant = "Accountant";

    /// <summary>เจ้าหน้าที่ทั่วไป</summary>
    public const string Staff = "Staff";

    /// <summary>ช่างซ่อมบำรุง</summary>
    public const string Technician = "Technician";

    /// <summary>ผู้เช่า เห็นเฉพาะข้อมูลของตนเอง</summary>
    public const string Tenant = "Tenant";

    /// <summary>รายชื่อบทบาททั้งหมด ใช้ตอน Seed ข้อมูลเริ่มต้น</summary>
    public static IReadOnlyList<string> All { get; } =
    [
        SuperAdmin,
        Owner,
        DormManager,
        Accountant,
        Staff,
        Technician,
        Tenant,
    ];
}
