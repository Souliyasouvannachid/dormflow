using DormFlow.Application.Features.Invoices;
using DormFlow.Application.Features.Reservations;
using DormFlow.Infrastructure.Options;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace DormFlow.Api.Configuration;

/// <summary>ตั้งค่า Background Job Server และ Recurring Job ทั้งหมด</summary>
public static class BackgroundJobConfiguration
{
    /// <summary>รหัสของ Recurring Job ที่หมดอายุการจองอัตโนมัติ</summary>
    private const string ExpireReservationsJobId = "expire-reservations";

    /// <summary>รหัสของ Recurring Job ที่ทำเครื่องหมายใบแจ้งหนี้เกินกำหนดชำระและคำนวณค่าปรับ</summary>
    private const string MarkOverdueInvoicesJobId = "mark-overdue-invoices";

    /// <summary>ทุก 15 นาที เพราะเวลาหมดอายุการกันห้อง (ExpiresAt) มีความละเอียดระดับนาที</summary>
    private const string ExpireReservationsCron = "*/15 * * * *";

    /// <summary>
    /// ลงทะเบียน Background Job Server
    /// </summary>
    /// <remarks>
    /// ข้ามการลงทะเบียนในสภาพแวดล้อม Testing หรือเมื่อปิดผ่านค่าตั้ง BackgroundJobs:Enabled
    /// ด้วยเงื่อนไขเดียวกับที่ Infrastructure.AddBackgroundJobStorage ใช้ข้ามการลงทะเบียน Storage
    /// (ต้องข้ามคู่กันเสมอ ไม่งั้น Server จะพยายาม resolve JobStorage ที่ไม่ได้ลงทะเบียนไว้)
    /// </remarks>
    /// <param name="services">Service collection ของแอปพลิเคชัน</param>
    /// <param name="configuration">Configuration ของแอปพลิเคชัน</param>
    /// <param name="environment">สภาพแวดล้อมโฮสต์ของแอปพลิเคชัน</param>
    public static IServiceCollection AddDormFlowBackgroundJobs(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        if (environment.IsEnvironment("Testing")
            || !configuration.GetValue("BackgroundJobs:Enabled", defaultValue: true))
        {
            return services;
        }

        services.AddHangfireServer();

        return services;
    }

    /// <summary>
    /// เปิดหน้า Dashboard และตั้งเวลา Recurring Job ทั้งหมด
    /// </summary>
    /// <remarks>
    /// ข้ามทั้งหมดในสภาพแวดล้อม Testing (Integration Test เตรียมฐานข้อมูลชั่วคราวของตัวเองอยู่แล้ว
    /// ไม่จำเป็นต้องมี Hangfire Schema หรือ Worker ทำงานคู่ขนานระหว่างทดสอบ) และเมื่อปิดผ่านค่าตั้ง
    /// BackgroundJobs:Enabled เอง — Dashboard เปิดเฉพาะสภาพแวดล้อมที่ไม่ใช่ Production เหมือนกับ OpenAPI/Scalar
    /// หากต้องการเปิดใน Production ให้ป้องกันด้วยการยืนยันตัวตนก่อนเสมอ
    /// </remarks>
    /// <param name="app">แอปพลิเคชันที่สร้างแล้ว</param>
    public static WebApplication UseDormFlowBackgroundJobs(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        if (app.Environment.IsEnvironment("Testing"))
        {
            return app;
        }

        BackgroundJobOptions options = app.Services.GetRequiredService<IOptions<BackgroundJobOptions>>().Value;
        if (!options.Enabled)
        {
            return app;
        }

        if (!app.Environment.IsProduction())
        {
            app.MapHangfireDashboard();
        }

        RecurringJob.AddOrUpdate<IReservationService>(
            ExpireReservationsJobId,
            service => service.ExpirePastDueReservationsAsync(CancellationToken.None),
            ExpireReservationsCron);

        RecurringJob.AddOrUpdate<IInvoiceService>(
            MarkOverdueInvoicesJobId,
            service => service.MarkOverdueInvoicesAsync(CancellationToken.None),
            Cron.Daily(1));

        return app;
    }
}
