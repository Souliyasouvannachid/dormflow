namespace DormFlow.Domain.Exceptions;

/// <summary>
/// ข้อผิดพลาดที่เกิดจากกฎทางธุรกิจใน Domain
/// ชั้น API จะแปลง Exception ตระกูลนี้เป็น HTTP 409 Conflict
/// </summary>
public class DomainException : Exception
{
    /// <summary>สร้าง Exception พร้อมข้อความเริ่มต้น</summary>
    public DomainException()
        : base("เกิดข้อผิดพลาดจากกฎทางธุรกิจ")
    {
    }

    /// <summary>สร้าง Exception พร้อมข้อความที่กำหนดเอง</summary>
    /// <param name="message">ข้อความอธิบายสาเหตุ</param>
    public DomainException(string message)
        : base(message)
    {
    }

    /// <summary>สร้าง Exception พร้อมข้อความและ Exception ต้นทาง</summary>
    /// <param name="message">ข้อความอธิบายสาเหตุ</param>
    /// <param name="innerException">Exception ต้นทาง</param>
    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
