using DormFlow.Api.Configuration;
using DormFlow.Api.Middleware;
using DormFlow.Api.Options;
using DormFlow.Application;
using DormFlow.Infrastructure;
using DormFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;

// Logger ชั่วคราวสำหรับดักข้อผิดพลาดที่เกิดก่อนอ่าน configuration เสร็จ
SerilogConfiguration.CreateBootstrapLogger();

try
{
    Log.Information("กำลังเริ่มต้น DormFlow API");

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.AddDormFlowSerilog();

    // ชั้น API
    builder.Services.AddDormFlowApi(builder.Configuration);
    builder.Services.AddDormFlowCors(builder.Configuration, builder.Environment);
    builder.Services.AddDormFlowRateLimiting(builder.Configuration);
    builder.Services.AddDormFlowOpenApi();

    // ชั้น Application และ Infrastructure
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddDormFlowHealthChecks();

    builder.Services.AddDormFlowAuthentication(builder.Configuration);

    // รับค่า IP และ scheme จริงเมื่อรันหลัง reverse proxy หรือ ingress
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();
    });

    WebApplication app = builder.Build();

    app.UseForwardedHeaders();

    // ต้องเป็นตัวแรกในไปป์ไลน์ เพื่อให้ครอบคลุมข้อผิดพลาดของ Middleware ที่เหลือทั้งหมด
    app.UseGlobalExceptionHandling();

    app.UseDormFlowRequestLogging();

    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
    }

    // ปิดการ redirect ไป HTTPS เมื่อรันในคอนเทนเนอร์ที่มี reverse proxy จัดการ TLS ให้แล้ว
    if (app.Configuration.GetValue("Https:EnableRedirection", defaultValue: true))
    {
        app.UseHttpsRedirection();
    }

    app.UseCors(CorsOptions.PolicyName);
    app.UseRateLimiter();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapDormFlowHealthChecks();
    app.UseDormFlowOpenApi();

    // ข้ามการปรับสคีมาและ Seed อัตโนมัติในสภาพแวดล้อม Testing
    // เพราะ Integration Test เตรียมฐานข้อมูลของตัวเองผ่าน DormFlowApiFactory อยู่แล้ว
    if (!app.Environment.IsEnvironment("Testing"))
    {
        await using AsyncServiceScope initializationScope = app.Services.CreateAsyncScope();
        DatabaseInitializer initializer = initializationScope.ServiceProvider
            .GetRequiredService<DatabaseInitializer>();
        await initializer.InitializeAsync();
    }

    Log.Information(
        "DormFlow API พร้อมทำงานในสภาพแวดล้อม {Environment}",
        app.Environment.EnvironmentName);

    await app.RunAsync();

    return 0;
}
catch (Exception exception)
{
    Log.Fatal(exception, "DormFlow API หยุดทำงานเพราะข้อผิดพลาดที่ไม่คาดคิดระหว่างเริ่มต้นระบบ");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}

/// <summary>
/// จุดเริ่มต้นของแอปพลิเคชัน ประกาศเป็น partial เพื่อให้ Integration Test อ้างถึงได้ผ่าน WebApplicationFactory
/// </summary>
public partial class Program
{
    /// <summary>ป้องกันการสร้างอินสแตนซ์ คลาสนี้มีไว้เป็นจุดอ้างอิงของ Test Host เท่านั้น</summary>
    protected Program()
    {
    }
}
