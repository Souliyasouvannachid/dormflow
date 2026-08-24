namespace DormFlow.Application.Common.Exceptions;

/// <summary>
/// ผู้ใช้ยืนยันตัวตนแล้วแต่ไม่มีสิทธิ์ทำรายการนี้ ชั้น API จะแปลงเป็น HTTP 403
/// </summary>
public class ForbiddenAccessException : Exception
{
    /// <summary>สร้าง Exception พร้อมข้อความเริ่มต้น</summary>
    public ForbiddenAccessException()
        : base("คุณไม่มีสิทธิ์ทำรายการนี้")
    {
    }

    /// <summary>สร้าง Exception พร้อมข้อความที่กำหนดเอง</summary>
    /// <param name="message">ข้อความอธิบายสาเหตุ</param>
    public ForbiddenAccessException(string message)
        : base(message)
    {
    }

    /// <summary>สร้าง Exception พร้อมข้อความและ Exception ต้นทาง</summary>
    /// <param name="message">ข้อความอธิบายสาเหตุ</param>
    /// <param name="innerException">Exception ต้นทาง</param>
    public ForbiddenAccessException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
