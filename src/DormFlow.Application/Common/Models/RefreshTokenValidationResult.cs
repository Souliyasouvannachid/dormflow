namespace DormFlow.Application.Common.Models;

/// <summary>สถานะผลการตรวจสอบ Refresh Token</summary>
public enum RefreshTokenValidationStatus
{
    /// <summary>ใช้ต่ออายุได้</summary>
    Valid = 1,

    /// <summary>ไม่พบโทเคนนี้ในระบบ</summary>
    NotFound = 2,

    /// <summary>โทเคนหมดอายุแล้ว</summary>
    Expired = 3,

    /// <summary>โทเคนถูกเพิกถอนไปแล้ว เช่น ผู้ใช้ออกจากระบบ</summary>
    Revoked = 4,

    /// <summary>
    /// ตรวจพบการนำโทเคนที่ถูกหมุนไปแล้วกลับมาใช้ซ้ำ
    /// ระบบจะเพิกถอนโทเคนทั้งหมดของผู้ใช้รายนั้นทันที
    /// </summary>
    ReuseDetected = 5,

    /// <summary>บัญชีของเจ้าของโทเคนถูกปิดใช้งานไปแล้ว</summary>
    AccountDisabled = 6,
}

/// <summary>ผลการตรวจสอบ Refresh Token</summary>
/// <param name="Status">สถานะผลลัพธ์</param>
/// <param name="TokenId">รหัสของแถวโทเคน มีค่าเฉพาะเมื่อพบโทเคนในระบบ</param>
/// <param name="UserId">เจ้าของโทเคน มีค่าเฉพาะเมื่อพบโทเคนในระบบ</param>
public sealed record RefreshTokenValidationResult(
    RefreshTokenValidationStatus Status,
    Guid? TokenId = null,
    Guid? UserId = null)
{
    /// <summary>สร้างผลลัพธ์ที่ใช้ต่ออายุได้</summary>
    /// <param name="tokenId">รหัสของแถวโทเคน</param>
    /// <param name="userId">เจ้าของโทเคน</param>
    public static RefreshTokenValidationResult Valid(Guid tokenId, Guid userId)
        => new(RefreshTokenValidationStatus.Valid, tokenId, userId);

    /// <summary>สร้างผลลัพธ์ที่ใช้ต่ออายุไม่ได้</summary>
    /// <param name="status">สาเหตุที่ใช้ไม่ได้</param>
    /// <param name="userId">เจ้าของโทเคน ถ้าทราบ</param>
    public static RefreshTokenValidationResult Invalid(
        RefreshTokenValidationStatus status,
        Guid? userId = null)
        => new(status, null, userId);
}
