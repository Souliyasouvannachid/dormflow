using System.Globalization;
using System.Text.Json;
using System.Threading.RateLimiting;
using DormFlow.Api.Options;
using DormFlow.Contracts.Common;
using Microsoft.AspNetCore.RateLimiting;

namespace DormFlow.Api.Configuration;

/// <summary>
/// ตั้งค่า Rate Limiting แบบ Fixed Window
/// </summary>
/// <remarks>
/// แบ่งโควตาตามผู้ใช้เมื่อยืนยันตัวตนแล้ว และตามหมายเลข IP เมื่อยังไม่ได้ยืนยันตัวตน
/// เพื่อไม่ให้ผู้ใช้รายเดียวกินโควตารวมของทั้งระบบ
/// </remarks>
public static class RateLimitingConfiguration
{
    private static readonly JsonSerializerOptions SerializerOptions = DormFlowJsonSerializerOptions.Default;

    /// <summary>ลงทะเบียน Rate Limiter กลางของระบบ</summary>
    /// <param name="services">Service collection ของแอปพลิเคชัน</param>
    /// <param name="configuration">Configuration ของแอปพลิเคชัน</param>
    public static IServiceCollection AddDormFlowRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<RateLimitingOptions>()
            .Bind(configuration.GetSection(RateLimitingOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        RateLimitingOptions rateLimitingOptions = new();
        configuration.GetSection(RateLimitingOptions.SectionName).Bind(rateLimitingOptions);

        services.AddRateLimiter(limiterOptions =>
        {
            limiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            limiterOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                if (!rateLimitingOptions.Enabled)
                {
                    return RateLimitPartition.GetNoLimiter("disabled");
                }

                // Health check ต้องเรียกได้เสมอ ไม่งั้น orchestrator จะเข้าใจผิดว่าระบบล่ม
                if (context.Request.Path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase))
                {
                    return RateLimitPartition.GetNoLimiter("health");
                }

                string partitionKey = context.User.Identity?.IsAuthenticated == true
                    ? $"user:{context.User.Identity.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown"}"
                    : $"ip:{context.Connection.RemoteIpAddress?.ToString() ?? "unknown"}";

                return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = rateLimitingOptions.PermitLimit,
                    Window = TimeSpan.FromSeconds(rateLimitingOptions.WindowSeconds),
                    QueueLimit = rateLimitingOptions.QueueLimit,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    AutoReplenishment = true,
                });
            });

            limiterOptions.OnRejected = async (context, cancellationToken) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString(CultureInfo.InvariantCulture);
                }

                context.HttpContext.Response.ContentType = "application/json; charset=utf-8";

                await context.HttpContext.Response.WriteAsJsonAsync(
                    ApiResponse.Fail(ApiMessages.TooManyRequests, context.HttpContext.TraceIdentifier),
                    SerializerOptions,
                    cancellationToken);
            };
        });

        return services;
    }
}
