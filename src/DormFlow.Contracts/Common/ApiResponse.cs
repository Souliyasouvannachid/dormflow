using System.Text.Json.Serialization;

namespace DormFlow.Contracts.Common;

/// <summary>
/// รูปแบบ Response มาตรฐานของ DormFlow API ที่ไม่มีข้อมูลส่งกลับ
/// </summary>
/// <example>
/// {
///   "success": true,
///   "message": "ดำเนินการสำเร็จ"
/// }
/// </example>
public class ApiResponse
{
    /// <summary>คำขอสำเร็จหรือไม่</summary>
    [JsonPropertyOrder(-30)]
    public bool Success { get; init; }

    /// <summary>ข้อความสรุปผลสำหรับแสดงต่อผู้ใช้</summary>
    [JsonPropertyOrder(-20)]
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// รายละเอียดข้อผิดพลาดรายฟิลด์ ส่งเฉพาะกรณี Validation ไม่ผ่าน
    /// key คือชื่อฟิลด์แบบ camelCase, value คือรายการข้อความผิดพลาดของฟิลด์นั้น
    /// </summary>
    [JsonPropertyOrder(10)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyDictionary<string, string[]>? Errors { get; init; }

    /// <summary>รหัสอ้างอิงคำขอ ใช้ค้นหา Log ฝั่งเซิร์ฟเวอร์ ส่งเฉพาะกรณีผิดพลาด</summary>
    [JsonPropertyOrder(20)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TraceId { get; init; }

    /// <summary>สร้าง Response สำเร็จที่ไม่มีข้อมูลส่งกลับ</summary>
    /// <param name="message">ข้อความสรุปผล</param>
    public static ApiResponse Ok(string message = ApiMessages.Success)
        => new() { Success = true, Message = message };

    /// <summary>สร้าง Response สำเร็จพร้อมข้อมูล</summary>
    /// <typeparam name="TData">ชนิดของข้อมูลที่ส่งกลับ</typeparam>
    /// <param name="data">ข้อมูลที่ส่งกลับ</param>
    /// <param name="message">ข้อความสรุปผล</param>
    public static ApiResponse<TData> Ok<TData>(TData data, string message = ApiMessages.Success)
        => new() { Success = true, Message = message, Data = data };

    /// <summary>สร้าง Response สำเร็จสำหรับข้อมูลแบบแบ่งหน้า</summary>
    /// <typeparam name="TItem">ชนิดของรายการในหน้า</typeparam>
    /// <param name="result">ผลลัพธ์แบบแบ่งหน้า</param>
    /// <param name="message">ข้อความสรุปผล</param>
    public static ApiResponse<PagedResult<TItem>> Paged<TItem>(
        PagedResult<TItem> result,
        string message = ApiMessages.Retrieved)
        => new() { Success = true, Message = message, Data = result };

    /// <summary>สร้าง Response ผิดพลาดที่ไม่มีรายละเอียดรายฟิลด์</summary>
    /// <param name="message">ข้อความอธิบายข้อผิดพลาด</param>
    /// <param name="traceId">รหัสอ้างอิงคำขอ</param>
    public static ApiResponse Fail(string message, string? traceId = null)
        => new() { Success = false, Message = message, TraceId = traceId };

    /// <summary>สร้าง Response ผิดพลาดพร้อมรายละเอียดรายฟิลด์</summary>
    /// <param name="message">ข้อความอธิบายข้อผิดพลาด</param>
    /// <param name="errors">รายละเอียดข้อผิดพลาดรายฟิลด์</param>
    /// <param name="traceId">รหัสอ้างอิงคำขอ</param>
    public static ApiResponse ValidationFailed(
        string message,
        IReadOnlyDictionary<string, string[]> errors,
        string? traceId = null)
        => new() { Success = false, Message = message, Errors = errors, TraceId = traceId };
}

/// <summary>
/// รูปแบบ Response มาตรฐานของ DormFlow API ที่มีข้อมูลส่งกลับ
/// </summary>
/// <typeparam name="TData">ชนิดของข้อมูลที่ส่งกลับ</typeparam>
/// <example>
/// {
///   "success": true,
///   "message": "ดึงข้อมูลสำเร็จ",
///   "data": { }
/// }
/// </example>
public class ApiResponse<TData> : ApiResponse
{
    /// <summary>ข้อมูลที่ส่งกลับ เป็น null เมื่อคำขอไม่สำเร็จ</summary>
    [JsonPropertyOrder(0)]
    public TData? Data { get; init; }
}
