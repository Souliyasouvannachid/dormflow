using System.Globalization;
using FluentValidation.Results;

namespace DormFlow.Application.Common.Exceptions;

/// <summary>
/// ข้อมูลที่ส่งเข้ามาไม่ผ่านการตรวจสอบ ชั้น API จะแปลงเป็น HTTP 400 พร้อมรายละเอียดรายฟิลด์
/// </summary>
/// <remarks>
/// ตั้งชื่อว่า ValidationFailedException เพื่อไม่ให้ชนกับ FluentValidation.ValidationException
/// ซึ่งจะทำให้ต้องใส่ namespace เต็มทุกครั้งที่อ้างถึง
/// </remarks>
public class ValidationFailedException : Exception
{
    private static readonly IReadOnlyDictionary<string, string[]> NoErrors =
        new Dictionary<string, string[]>(StringComparer.Ordinal);

    /// <summary>สร้าง Exception พร้อมข้อความเริ่มต้นและไม่มีรายละเอียดรายฟิลด์</summary>
    public ValidationFailedException()
        : base("ข้อมูลไม่ถูกต้อง")
    {
        Errors = NoErrors;
    }

    /// <summary>สร้าง Exception พร้อมข้อความที่กำหนดเอง</summary>
    /// <param name="message">ข้อความอธิบายสาเหตุ</param>
    public ValidationFailedException(string message)
        : base(message)
    {
        Errors = NoErrors;
    }

    /// <summary>สร้าง Exception พร้อมข้อความและ Exception ต้นทาง</summary>
    /// <param name="message">ข้อความอธิบายสาเหตุ</param>
    /// <param name="innerException">Exception ต้นทาง</param>
    public ValidationFailedException(string message, Exception innerException)
        : base(message, innerException)
    {
        Errors = NoErrors;
    }

    /// <summary>สร้าง Exception จากผลการตรวจสอบของ FluentValidation</summary>
    /// <param name="failures">รายการข้อผิดพลาดที่ตรวจพบ</param>
    public ValidationFailedException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        ArgumentNullException.ThrowIfNull(failures);

        Errors = failures
            .GroupBy(failure => ToCamelCase(failure.PropertyName), StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.Select(failure => failure.ErrorMessage).Distinct(StringComparer.Ordinal).ToArray(),
                StringComparer.Ordinal);
    }

    /// <summary>
    /// รายละเอียดข้อผิดพลาดรายฟิลด์
    /// key คือชื่อฟิลด์แบบ camelCase ให้ตรงกับรูปแบบ JSON ที่ Client ส่งมา
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    private static string ToCamelCase(string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            return propertyName;
        }

        // รองรับชื่อซ้อน เช่น Address.PostalCode -> address.postalCode
        string[] segments = propertyName.Split('.');
        for (int i = 0; i < segments.Length; i++)
        {
            string segment = segments[i];
            if (segment.Length > 0 && char.IsUpper(segment[0]))
            {
                segments[i] = string.Create(CultureInfo.InvariantCulture, $"{char.ToLowerInvariant(segment[0])}{segment[1..]}");
            }
        }

        return string.Join('.', segments);
    }
}
