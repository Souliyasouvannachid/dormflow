namespace DormFlow.Contracts.Authentication;

/// <summary>ข้อมูลสำหรับออกจากระบบ</summary>
public class LogoutRequest
{
    /// <summary>Refresh Token ที่ต้องการเพิกถอน</summary>
    public string RefreshToken { get; set; } = string.Empty;
}
