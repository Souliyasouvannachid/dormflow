namespace DormFlow.Api.Middleware;

/// <summary>ส่วนขยายสำหรับต่อ Middleware ของ DormFlow เข้ากับไปป์ไลน์</summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// เพิ่มตัวดักจับข้อผิดพลาดกลางของระบบ
    /// ต้องเรียกเป็นตัวแรกของไปป์ไลน์เพื่อให้ครอบคลุม Middleware ที่เหลือทั้งหมด
    /// </summary>
    /// <param name="app">ไปป์ไลน์ของแอปพลิเคชัน</param>
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        return app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }
}
