using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using DormFlow.Application.Common.Abstractions;
using Serilog;
using Serilog.Events;

namespace DormFlow.Api.Configuration;

/// <summary>
/// ตั้งค่า Serilog ทั้งตอนบูตและตอนบันทึก HTTP Request
/// </summary>
/// <remarks>
/// ระบบไม่บันทึก Request Body หรือ Response Body ลง Log เลย
/// เพราะเส้นทางอย่าง login และ reset-password มีรหัสผ่านและโทเคนอยู่ในเนื้อหา
/// </remarks>
public static class SerilogConfiguration
{
    /// <summary>
    /// สร้าง Logger ชั่วคราวก่อนอ่าน configuration
    /// ใช้เพื่อให้ข้อผิดพลาดที่เกิดระหว่างการตั้งค่ายังถูกบันทึกไว้
    /// </summary>
    public static void CreateBootstrapLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .CreateBootstrapLogger();
    }

    /// <summary>ผูก Serilog เข้ากับ Host และอ่านค่าจาก configuration section ชื่อ Serilog</summary>
    /// <param name="builder">ตัวสร้างแอปพลิเคชัน</param>
    public static WebApplicationBuilder AddDormFlowSerilog(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());

        return builder;
    }

    /// <summary>
    /// บันทึกสรุปหนึ่งบรรทัดต่อหนึ่งคำขอ พร้อมข้อมูลบริบทที่ใช้สืบค้นย้อนหลังได้
    /// </summary>
    /// <param name="app">ไปป์ไลน์ของแอปพลิเคชัน</param>
    public static IApplicationBuilder UseDormFlowRequestLogging(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        return app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "{RequestMethod} {RequestPath} ตอบกลับ {StatusCode} ใช้เวลา {Elapsed:0.0000} ms";

            options.GetLevel = GetLogLevel;
            options.EnrichDiagnosticContext = EnrichDiagnosticContext;
        });
    }

    /// <summary>
    /// ไม่บันทึกการเรียก Health Check ที่ผ่านฉลุยเป็นระดับ Information
    /// เพราะ orchestrator เรียกถี่มากจนกลบ Log ที่มีความหมายจริง
    /// </summary>
    private static LogEventLevel GetLogLevel(HttpContext context, double elapsedMilliseconds, Exception? exception)
    {
        if (exception is not null || context.Response.StatusCode >= StatusCodes.Status500InternalServerError)
        {
            return LogEventLevel.Error;
        }

        if (context.Response.StatusCode >= StatusCodes.Status400BadRequest)
        {
            return LogEventLevel.Warning;
        }

        bool isHealthCheck = context.Request.Path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase);

        return isHealthCheck ? LogEventLevel.Verbose : LogEventLevel.Information;
    }

    private static void EnrichDiagnosticContext(IDiagnosticContext diagnosticContext, HttpContext context)
    {
        diagnosticContext.Set("TraceId", Activity.Current?.Id ?? context.TraceIdentifier);
        diagnosticContext.Set("RemoteIp", context.Connection.RemoteIpAddress?.ToString() ?? "unknown");
        diagnosticContext.Set("UserAgent", context.Request.Headers.UserAgent.ToString());

        ClaimsPrincipal user = context.User;
        if (user.Identity?.IsAuthenticated != true)
        {
            return;
        }

        string? userId = user.FindFirstValue(DormFlowClaimTypes.UserId)
            ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!string.IsNullOrWhiteSpace(userId))
        {
            diagnosticContext.Set("UserId", userId);
        }

        string? organizationId = user.FindFirstValue(DormFlowClaimTypes.OrganizationId);
        if (!string.IsNullOrWhiteSpace(organizationId))
        {
            diagnosticContext.Set("OrganizationId", organizationId);
        }

        string[] dormitoryIds = user.FindAll(DormFlowClaimTypes.DormitoryId)
            .Select(claim => claim.Value)
            .ToArray();

        if (dormitoryIds.Length > 0)
        {
            diagnosticContext.Set("DormitoryIds", dormitoryIds);
        }
    }
}
