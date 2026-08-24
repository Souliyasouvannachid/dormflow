using System.Text.Json;
using DormFlow.Contracts.Common;
using DormFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DormFlow.Api.Configuration;

/// <summary>
/// ตั้งค่า Health Check ของ API และ PostgreSQL
/// </summary>
/// <remarks>
/// แยกเป็นสามเส้นทางตามการใช้งานของ orchestrator
/// live ใช้ตัดสินว่าควรรีสตาร์ตคอนเทนเนอร์หรือไม่ จึงต้องไม่ขึ้นกับฐานข้อมูล
/// ready ใช้ตัดสินว่าควรส่งทราฟฟิกเข้ามาหรือยัง จึงต้องตรวจฐานข้อมูลด้วย
/// </remarks>
public static class HealthCheckConfiguration
{
    /// <summary>แท็กของการตรวจสอบที่ใช้ตัดสินความพร้อมรับทราฟฟิก</summary>
    public const string ReadyTag = "ready";

    /// <summary>แท็กของการตรวจสอบที่ใช้ตัดสินว่ากระบวนการยังมีชีวิตอยู่</summary>
    public const string LiveTag = "live";

    private static readonly JsonSerializerOptions SerializerOptions = DormFlowJsonSerializerOptions.Default;

    /// <summary>ลงทะเบียนรายการตรวจสอบสุขภาพของระบบ</summary>
    /// <param name="services">Service collection ของแอปพลิเคชัน</param>
    public static IServiceCollection AddDormFlowHealthChecks(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("API ทำงานอยู่"), tags: [LiveTag])
            .AddDbContextCheck<DormFlowDbContext>(
                name: "postgresql",
                failureStatus: HealthStatus.Unhealthy,
                tags: [ReadyTag]);

        return services;
    }

    /// <summary>เปิดเส้นทาง /health, /health/ready และ /health/live</summary>
    /// <param name="app">แอปพลิเคชันที่สร้างแล้ว</param>
    public static WebApplication MapDormFlowHealthChecks(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        HealthCheckOptions allChecks = new() { ResponseWriter = WriteResponseAsync };

        HealthCheckOptions readyChecks = new()
        {
            Predicate = registration => registration.Tags.Contains(ReadyTag),
            ResponseWriter = WriteResponseAsync,
        };

        HealthCheckOptions liveChecks = new()
        {
            Predicate = registration => registration.Tags.Contains(LiveTag),
            ResponseWriter = WriteResponseAsync,
        };

        app.MapHealthChecks("/health", allChecks).AllowAnonymous();
        app.MapHealthChecks("/health/ready", readyChecks).AllowAnonymous();
        app.MapHealthChecks("/health/live", liveChecks).AllowAnonymous();

        return app;
    }

    /// <summary>
    /// เขียนผลการตรวจสอบเป็น JSON
    /// ไม่ส่งข้อความของ Exception ออกไป เพราะอาจมีรายละเอียดการเชื่อมต่อฐานข้อมูลปนอยู่
    /// </summary>
    private static Task WriteResponseAsync(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var payload = new
        {
            status = report.Status.ToString(),
            totalDurationMs = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                durationMs = entry.Value.Duration.TotalMilliseconds,
                description = entry.Value.Description,
            }),
        };

        return context.Response.WriteAsJsonAsync(payload, SerializerOptions, context.RequestAborted);
    }
}
