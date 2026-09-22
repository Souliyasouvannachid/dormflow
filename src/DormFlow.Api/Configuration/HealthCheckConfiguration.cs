using System.Text.Json;
using DormFlow.Contracts.Common;
using DormFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DormFlow.Api.Configuration;

/// <summary>
/// ຕັ້ງຄ່າ Health Check ຂອງ API ແລະ PostgreSQL
/// </summary>
/// <remarks>
/// ແຍກອອກເປັນສາມເສັ້ນທາງຕາມການນໍາໃຊ້ orchestrator
/// live ໃຊ້ເພື່ອຕັດສິນໃຈວ່າຖັງຄວນປິດເປີດໃໝ່ຫຼືບໍ່. ດັ່ງນັ້ນ, ມັນຕ້ອງເປັນເອກະລາດຂອງຖານຂໍ້ມູນ
/// ready ໃຊ້ເພື່ອຕັດສິນໃຈວ່າການຈະລາຈອນຄວນຈະຖືກສົ່ງຫຼືບໍ່. ດັ່ງນັ້ນ, ຖານຂໍ້ມູນຕ້ອງໄດ້ຮັບການກວດສອບເຊັ່ນດຽວກັນ
/// </remarks>
public static class HealthCheckConfiguration
{
    /// <summary>ປ້າຍກວດກາທີ່ໃຊ້ເພື່ອກໍານົດຄວາມພ້ອມຂອງການຈະລາຈອນ</summary>
    public const string ReadyTag = "ready";

    /// <summary>ແທັກ checksum ທີ່ໃຊ້ເພື່ອກໍານົດວ່າຂະບວນການມີຊີວິດຢູ່ຫຼືບໍ່</summary>
    public const string LiveTag = "live";

    private static readonly JsonSerializerOptions SerializerOptions = DormFlowJsonSerializerOptions.Default;

    /// <summary>ລົງທະບຽນສໍາລັບລາຍການກວດສຸຂະພາບຂອງລະບົບ</summary>
    /// <param name="services">Service collection ຂອງຄໍາຮ້ອງສະຝັກ</param>
    public static IServiceCollection AddDormFlowHealthChecks(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("API ກຳລັງເຮັດວຽກ"), tags: [LiveTag])
            .AddDbContextCheck<DormFlowDbContext>(
                name: "postgresql",
                failureStatus: HealthStatus.Unhealthy,
                tags: [ReadyTag]);

        return services;
    }

    /// <summary>ເສັ້ນທາງເປີດ /health, /health/ready ແລະ /health/live</summary>
    /// <param name="app">ແອັບພລິເຄຊັນທີ່ສ້າງຂຶ້ນ</param>
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
    /// ຂຽນຜົນການກວດກາເປັນ JSON
    /// ຢ່າສົ່ງຂໍ້ຄວາມ Exception ໄປເພາະວ່າອາດຈະມີລາຍລະອຽດການເຊື່ອມຕໍ່ຖານຂໍ້ມູນປະສົມຢູ່ໃນ
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
