namespace DormFlow.Contracts.Users;

/// <summary>คำขอแก้ไขข้อมูลโปรไฟล์ของผู้ใช้</summary>
public class UpdateUserRequest
{
    /// <summary>ชื่อ</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>นามสกุล</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>เบอร์โทรศัพท์</summary>
    public string? Phone { get; set; }

    /// <summary>ที่อยู่ของรูปโปรไฟล์</summary>
    public string? ProfileImage { get; set; }
}
