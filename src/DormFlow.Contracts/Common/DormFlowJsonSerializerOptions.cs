using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace DormFlow.Contracts.Common;

/// <summary>
/// ค่าตั้งการแปลง JSON ที่ใช้ร่วมกันทุกจุดที่ระบบเขียน Response
/// </summary>
/// <remarks>
/// รวมไว้ที่เดียวเพื่อให้ Response จาก Controller, Middleware, Health Check และ Rate Limiter
/// มีรูปแบบเดียวกันทั้งหมด
/// </remarks>
public static class DormFlowJsonSerializerOptions
{
    /// <summary>ค่าตั้งมาตรฐานที่พร้อมใช้งาน</summary>
    public static JsonSerializerOptions Default { get; } = CreateDefault();

    /// <summary>สร้างค่าตั้งมาตรฐานชุดใหม่ที่ยังแก้ไขเพิ่มได้</summary>
    public static JsonSerializerOptions CreateDefault()
    {
        JsonSerializerOptions options = new(JsonSerializerDefaults.Web);
        Configure(options);
        options.MakeReadOnly(populateMissingResolver: true);
        return options;
    }

    /// <summary>ใส่ค่าตั้งมาตรฐานของ DormFlow ลงในอ็อบเจกต์ที่มีอยู่แล้ว</summary>
    /// <param name="options">ค่าตั้งที่ต้องการปรับ เช่นของ ASP.NET Core MVC</param>
    public static void Configure(JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

        // ปล่อยให้อักษรไทยออกไปเป็นตัวอักษรจริงแทน ก ทำให้ Response อ่านง่ายและขนาดเล็กลง
        // ใช้ได้อย่างปลอดภัยเพราะ API ตอบเป็น application/json เสมอ ไม่เคยฝังผลลัพธ์ลงใน HTML
        options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Thai);

        if (!options.Converters.Any(converter => converter is JsonStringEnumConverter))
        {
            options.Converters.Add(new JsonStringEnumConverter());
        }
    }
}
