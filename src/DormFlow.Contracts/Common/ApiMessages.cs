namespace DormFlow.Contracts.Common;

/// <summary>
/// ข้อความมาตรฐานของ API Response ทั้งระบบ
/// รวมไว้ที่เดียวเพื่อให้ข้อความที่ผู้ใช้เห็นสอดคล้องกันทุก Endpoint
/// </summary>
public static class ApiMessages
{
    /// <summary>ดำเนินการสำเร็จ (ใช้กับ Create / Update / Delete / Change Status)</summary>
    public const string Success = "ดำเนินการสำเร็จ";

    /// <summary>ดึงข้อมูลสำเร็จ (ใช้กับ Query)</summary>
    public const string Retrieved = "ดึงข้อมูลสำเร็จ";

    /// <summary>สร้างข้อมูลสำเร็จ</summary>
    public const string Created = "สร้างข้อมูลสำเร็จ";

    /// <summary>แก้ไขข้อมูลสำเร็จ</summary>
    public const string Updated = "แก้ไขข้อมูลสำเร็จ";

    /// <summary>ลบข้อมูลสำเร็จ</summary>
    public const string Deleted = "ลบข้อมูลสำเร็จ";

    /// <summary>ข้อมูลที่ส่งมาไม่ผ่านการตรวจสอบ (HTTP 400)</summary>
    public const string ValidationFailed = "ข้อมูลไม่ถูกต้อง";

    /// <summary>ไม่พบข้อมูลที่ร้องขอ (HTTP 404)</summary>
    public const string NotFound = "ไม่พบข้อมูลที่ต้องการ";

    /// <summary>ยังไม่ได้เข้าสู่ระบบหรือ Token ไม่ถูกต้อง (HTTP 401)</summary>
    public const string Unauthorized = "กรุณาเข้าสู่ระบบ";

    /// <summary>เข้าสู่ระบบแล้วแต่ไม่มีสิทธิ์ (HTTP 403)</summary>
    public const string Forbidden = "คุณไม่มีสิทธิ์ทำรายการนี้";

    /// <summary>ข้อมูลขัดแย้งกับสถานะปัจจุบัน (HTTP 409)</summary>
    public const string Conflict = "ข้อมูลขัดแย้งกับข้อมูลที่มีอยู่ในระบบ";

    /// <summary>ข้อมูลถูกแก้ไขโดยผู้ใช้อื่นระหว่างทาง (HTTP 409)</summary>
    public const string ConcurrencyConflict = "ข้อมูลถูกแก้ไขโดยผู้ใช้อื่น กรุณาโหลดข้อมูลใหม่แล้วลองอีกครั้ง";

    /// <summary>เรียกใช้งานถี่เกินกำหนด (HTTP 429)</summary>
    public const string TooManyRequests = "มีการเรียกใช้งานถี่เกินกำหนด กรุณาลองใหม่อีกครั้งในภายหลัง";

    /// <summary>ข้อผิดพลาดที่ไม่คาดคิดของระบบ (HTTP 500)</summary>
    public const string InternalServerError = "เกิดข้อผิดพลาดภายในระบบ กรุณาลองใหม่อีกครั้ง";

    /// <summary>คำขอถูกยกเลิกโดยผู้เรียก (HTTP 499)</summary>
    public const string RequestCancelled = "คำขอถูกยกเลิก";
}
