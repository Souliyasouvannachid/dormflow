namespace DormFlow.Infrastructure.Options;

/// <summary>
/// ข้อมูลผู้ดูแลระบบเริ่มต้นที่ใช้ตอน Seed
/// </summary>
/// <remarks>
/// อ่านจาก environment variable โดยตรงตามชื่อที่กำหนดไว้ในเอกสารระบบ
/// ถ้าไม่ได้กำหนดครบ ระบบจะข้ามการสร้างบัญชีผู้ดูแลและบันทึกคำเตือนไว้
/// จะไม่มีการสร้างบัญชีที่ฝังรหัสผ่านไว้ในโค้ดเด็ดขาด
/// </remarks>
public sealed class SeedOptions
{
    /// <summary>ชื่อ environment variable ของอีเมลผู้ดูแลระบบ</summary>
    public const string AdminEmailVariable = "SEED_ADMIN_EMAIL";

    /// <summary>ชื่อ environment variable ของรหัสผ่านผู้ดูแลระบบ</summary>
    public const string AdminPasswordVariable = "SEED_ADMIN_PASSWORD";

    /// <summary>ชื่อ environment variable ของชื่อผู้ดูแลระบบ</summary>
    public const string AdminFirstNameVariable = "SEED_ADMIN_FIRST_NAME";

    /// <summary>ชื่อ environment variable ของนามสกุลผู้ดูแลระบบ</summary>
    public const string AdminLastNameVariable = "SEED_ADMIN_LAST_NAME";

    /// <summary>อีเมลของผู้ดูแลระบบเริ่มต้น</summary>
    public string? AdminEmail { get; set; }

    /// <summary>รหัสผ่านของผู้ดูแลระบบเริ่มต้น</summary>
    public string? AdminPassword { get; set; }

    /// <summary>ชื่อของผู้ดูแลระบบเริ่มต้น</summary>
    public string? AdminFirstName { get; set; }

    /// <summary>นามสกุลของผู้ดูแลระบบเริ่มต้น</summary>
    public string? AdminLastName { get; set; }

    /// <summary>มีข้อมูลครบพอที่จะสร้างบัญชีผู้ดูแลระบบหรือไม่</summary>
    public bool HasAdminCredentials =>
        !string.IsNullOrWhiteSpace(AdminEmail) && !string.IsNullOrWhiteSpace(AdminPassword);
}
