using DormFlow.Api.OpenApi;
using DormFlow.Api.Options;
using Scalar.AspNetCore;

namespace DormFlow.Api.Configuration;

/// <summary>ตั้งค่าเอกสาร OpenAPI และหน้าเอกสารแบบโต้ตอบ</summary>
public static class OpenApiConfiguration
{
    /// <summary>ชื่อเอกสารของ API เวอร์ชัน 1</summary>
    public const string V1DocumentName = "v1";

    /// <summary>ลงทะเบียนการสร้างเอกสาร OpenAPI</summary>
    /// <param name="services">Service collection ของแอปพลิเคชัน</param>
    public static IServiceCollection AddDormFlowOpenApi(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // การเรียก services.AddOpenApi(...) โดยตรงจำเป็นต่อการดึง XML Comments เข้าเอกสาร
        // เพราะ source generator ของ .NET 10 ทำงานด้วยการ intercept การเรียกนี้ในโค้ดของแอปพลิเคชันเอง
        services.AddOpenApi(V1DocumentName, options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        return services;
    }

    /// <summary>
    /// เปิดเส้นทางเอกสาร OpenAPI และหน้าเอกสารแบบโต้ตอบ
    /// </summary>
    /// <param name="app">แอปพลิเคชันที่สร้างแล้ว</param>
    /// <remarks>
    /// เปิดเฉพาะสภาพแวดล้อมที่ไม่ใช่ Production
    /// หากต้องการเปิดใน Production ให้ป้องกันด้วยการยืนยันตัวตนก่อนเสมอ
    /// </remarks>
    public static WebApplication UseDormFlowOpenApi(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        if (app.Environment.IsProduction())
        {
            return app;
        }

        app.MapOpenApi().WithDocumentPerVersion();

        ApiOptions apiOptions = app.Services
            .GetRequiredService<Microsoft.Extensions.Options.IOptions<ApiOptions>>()
            .Value;

        app.MapScalarApiReference(options =>
        {
            options.Title = apiOptions.Title;
            options.WithTheme(ScalarTheme.Purple);
        });

        return app;
    }
}
