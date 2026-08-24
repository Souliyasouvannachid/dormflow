namespace DormFlow.Application.Common.Exceptions;

/// <summary>
/// คำขอขัดกับสถานะปัจจุบันของข้อมูล เช่น รหัสซ้ำ ชั้น API จะแปลงเป็น HTTP 409
/// </summary>
public class ConflictException : Exception
{
    /// <summary>สร้าง Exception พร้อมข้อความเริ่มต้น</summary>
    public ConflictException()
        : base("ข้อมูลขัดแย้งกับข้อมูลที่มีอยู่ในระบบ")
    {
    }

    /// <summary>สร้าง Exception พร้อมข้อความที่กำหนดเอง</summary>
    /// <param name="message">ข้อความอธิบายสาเหตุ</param>
    public ConflictException(string message)
        : base(message)
    {
    }

    /// <summary>สร้าง Exception พร้อมข้อความและ Exception ต้นทาง</summary>
    /// <param name="message">ข้อความอธิบายสาเหตุ</param>
    /// <param name="innerException">Exception ต้นทาง</param>
    public ConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
