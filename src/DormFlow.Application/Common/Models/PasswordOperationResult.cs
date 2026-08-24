namespace DormFlow.Application.Common.Models;

/// <summary>ผลลัพธ์ของการดำเนินการเกี่ยวกับรหัสผ่าน</summary>
/// <param name="Succeeded">สำเร็จหรือไม่</param>
/// <param name="Errors">ข้อความอธิบายสาเหตุที่ไม่สำเร็จ</param>
public sealed record PasswordOperationResult(bool Succeeded, IReadOnlyList<string> Errors)
{
    /// <summary>ผลลัพธ์ที่สำเร็จ</summary>
    public static PasswordOperationResult Success { get; } = new(true, []);

    /// <summary>สร้างผลลัพธ์ที่ไม่สำเร็จ</summary>
    /// <param name="errors">ข้อความอธิบายสาเหตุ</param>
    public static PasswordOperationResult Failed(params string[] errors) => new(false, errors);
}
