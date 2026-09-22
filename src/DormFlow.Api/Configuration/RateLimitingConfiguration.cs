using System.Globalization;
using System.Text.Json;
using System.Threading.RateLimiting;
using DormFlow.Api.Options;
using DormFlow.Contracts.Common;
using Microsoft.AspNetCore.RateLimiting;

namespace DormFlow.Api.Configuration;

/// <summary>
/// ຕັ້ງຄ່າ Rate Limiting ຕົວແບບ Fixed Window
/// </summary>
/// <remarks>
/// ໂຄຕ້າໄດ້ຖືກແບ່ງອອກລະຫວ່າງຜູ້ໃຊ້ເມື່ອພວກເຂົາໄດ້ຮັບການຢັ້ງຢືນ. ແລະໂດຍຕົວເລກ IP ເມື່ອເຈົ້າຍັງບໍ່ໄດ້ຢືນຢັນຕົວຕົນຂອງເຈົ້າເທື່ອ
/// ນີ້ແມ່ນເພື່ອປ້ອງກັນບໍ່ໃຫ້ຜູ້ໃຊ້ດຽວຈາກການກິນເຖິງໂຄຕ້າທັງຝົດຂອງລະບົບທັງຝົດ
/// </remarks>
public static class RateLimitingConfiguration
{
    private static readonly JsonSerializerOptions SerializerOptions = DormFlowJsonSerializerOptions.Default;

    /// <summary>ລົງທະບຽນ Rate Limiter ກາງຂອງລະບົບ</summary>
    /// <param name="services">Service collection ຂອງຄໍາຮ້ອງສະຝັກ</param>
    /// <param name="configuration">Configuration ຂອງຄໍາຮ້ອງສະຝັກ</param>
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

                // Health check ທ່ານສະເຝີສາມາດໂທຫາ, ຖ້າບໍ່ດັ່ງນັ້ນ orchestrator ມັນຈະຖືກເຂົ້າໃຈຜິດວ່າລະບົບຫຼຸດລົງ
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
