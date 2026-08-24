namespace DormFlow.Domain.Enums;

/// <summary>สาเหตุที่ Refresh Token ถูกเพิกถอน</summary>
public enum RefreshTokenRevocationReason
{
    /// <summary>ถูกแทนที่ด้วยโทเคนใหม่ตามกลไก Token Rotation</summary>
    ReplacedByNewToken = 1,

    /// <summary>ผู้ใช้ออกจากระบบ</summary>
    Logout = 2,

    /// <summary>ผู้ใช้หรือผู้ดูแลสั่งเพิกถอน</summary>
    RevokedManually = 3,

    /// <summary>ตรวจพบการใช้โทเคนที่ถูกหมุนไปแล้วซ้ำ จึงเพิกถอนทั้งสายโทเคน</summary>
    ReuseDetected = 4,

    /// <summary>ผู้ใช้เปลี่ยนหรือตั้งรหัสผ่านใหม่</summary>
    PasswordChanged = 5,

    /// <summary>บัญชีถูกระงับหรือปิดใช้งาน</summary>
    AccountDisabled = 6,
}
