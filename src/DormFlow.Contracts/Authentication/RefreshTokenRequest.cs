namespace DormFlow.Contracts.Authentication;

/// <summary>ข้อมูลสำหรับขอ Access Token ใบใหม่</summary>
public class RefreshTokenRequest
{
    /// <summary>Refresh Token ที่ได้รับตอนเข้าสู่ระบบหรือจากการต่ออายุครั้งก่อน</summary>
    public string RefreshToken { get; set; } = string.Empty;
}
