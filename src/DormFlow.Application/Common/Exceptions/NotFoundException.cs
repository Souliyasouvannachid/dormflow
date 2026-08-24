namespace DormFlow.Application.Common.Exceptions;

/// <summary>
/// ไม่พบข้อมูลที่ร้องขอ หรือผู้ใช้ไม่มีสิทธิ์เห็นข้อมูลนั้น
/// ชั้น API จะแปลงเป็น HTTP 404
/// </summary>
/// <remarks>
/// เมื่อข้อมูลอยู่นอกขอบเขตองค์กรของผู้ใช้ ให้ตอบ 404 แทน 403
/// เพื่อไม่ให้ผู้เรียกเดาได้ว่ามีข้อมูลนั้นอยู่จริงในองค์กรอื่น
/// </remarks>
public class NotFoundException : Exception
{
    /// <summary>สร้าง Exception พร้อมข้อความเริ่มต้น</summary>
    public NotFoundException()
        : base("ไม่พบข้อมูลที่ต้องการ")
    {
    }

    /// <summary>สร้าง Exception พร้อมข้อความที่กำหนดเอง</summary>
    /// <param name="message">ข้อความอธิบายสาเหตุ</param>
    public NotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>สร้าง Exception พร้อมข้อความและ Exception ต้นทาง</summary>
    /// <param name="message">ข้อความอธิบายสาเหตุ</param>
    /// <param name="innerException">Exception ต้นทาง</param>
    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>สร้าง Exception จากชื่อ Entity และคีย์ที่ค้นหา</summary>
    /// <param name="entityName">ชื่อ Entity เช่น Room</param>
    /// <param name="key">คีย์ที่ใช้ค้นหา</param>
    public static NotFoundException For(string entityName, object key)
        => new($"ไม่พบข้อมูล {entityName} ที่อ้างอิง {key}");
}
