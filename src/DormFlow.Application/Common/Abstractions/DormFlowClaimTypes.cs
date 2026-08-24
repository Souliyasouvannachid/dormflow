namespace DormFlow.Application.Common.Abstractions;

/// <summary>
/// ชื่อ Claim ที่ DormFlow ใส่ไว้ใน Access Token
/// ใช้ชุดเดียวกันทั้งตอนออกโทเคนและตอนอ่านข้อมูลผู้ใช้ปัจจุบัน
/// </summary>
public static class DormFlowClaimTypes
{
    /// <summary>รหัสผู้ใช้</summary>
    public const string UserId = "sub";

    /// <summary>องค์กรที่ผู้ใช้สังกัด</summary>
    public const string OrganizationId = "org_id";

    /// <summary>หอพักที่ผู้ใช้ถูกมอบหมาย ปรากฏได้หลายครั้งในโทเคนเดียว</summary>
    public const string DormitoryId = "dorm_id";

    /// <summary>รหัสสิทธิ์ ปรากฏได้หลายครั้งในโทเคนเดียว</summary>
    public const string Permission = "perm";

    /// <summary>ชื่อของผู้ใช้</summary>
    public const string FirstName = "given_name";

    /// <summary>นามสกุลของผู้ใช้</summary>
    public const string LastName = "family_name";

    /// <summary>รหัสอ้างอิงของ Access Token ใช้ผูกกับ Refresh Token</summary>
    public const string JwtId = "jti";
}
