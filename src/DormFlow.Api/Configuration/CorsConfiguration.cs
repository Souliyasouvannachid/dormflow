using DormFlow.Api.Options;
using Microsoft.Extensions.Options;

namespace DormFlow.Api.Configuration;

/// <summary>ตั้งค่า CORS จาก configuration เท่านั้น</summary>
public static class CorsConfiguration
{
    /// <summary>
    /// ลงทะเบียน CORS policy ตามรายการ Origin ที่กำหนดไว้ใน configuration
    /// </summary>
    /// <param name="services">Service collection ของแอปพลิเคชัน</param>
    /// <param name="configuration">Configuration ของแอปพลิเคชัน</param>
    /// <param name="environment">สภาพแวดล้อมที่กำลังทำงาน</param>
    /// <exception cref="InvalidOperationException">
    /// เมื่อรันใน Production แต่ไม่ได้ระบุ Origin ที่อนุญาตไว้เลย
    /// ระบบจะไม่ยอมสตาร์ต แทนที่จะเผลอเปิดกว้างให้ทุก Origin
    /// </exception>
    public static IServiceCollection AddDormFlowCors(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        services.AddOptions<CorsOptions>()
            .Bind(configuration.GetSection(CorsOptions.SectionName))
            .ValidateDataAnnotations();

        CorsOptions corsOptions = new();
        configuration.GetSection(CorsOptions.SectionName).Bind(corsOptions);

        string[] allowedOrigins = corsOptions.AllowedOrigins
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Select(origin => origin.TrimEnd('/'))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (allowedOrigins.Length == 0 && environment.IsProduction())
        {
            throw new InvalidOperationException(
                "ไม่พบรายการ Cors:AllowedOrigins สำหรับ Production " +
                "กำหนดผ่าน environment variable ชื่อ Cors__AllowedOrigins__0 เป็นต้นไป");
        }

        services.AddCors(options => options.AddPolicy(CorsOptions.PolicyName, policy =>
        {
            if (allowedOrigins.Length == 0)
            {
                // เกิดได้เฉพาะนอก Production เท่านั้น เพราะกรณี Production ถูกดักไว้ด้านบนแล้ว
                policy.SetIsOriginAllowed(_ => true);
            }
            else
            {
                policy.WithOrigins(allowedOrigins);
            }

            policy.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithExposedHeaders("X-Trace-Id");
        }));

        return services;
    }

    /// <summary>อ่านรายการ Origin ที่อนุญาตหลังการลงทะเบียน ใช้ตอนบันทึก Log ตอนสตาร์ต</summary>
    /// <param name="services">Service provider ของแอปพลิเคชัน</param>
    public static IReadOnlyList<string> GetAllowedOrigins(this IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return [.. services.GetRequiredService<IOptions<CorsOptions>>().Value.AllowedOrigins];
    }
}
