using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using DormFlow.Application.Common.Abstractions;
using Serilog;
using Serilog.Events;

namespace DormFlow.Api.Configuration;

/// <summary>
/// ຕັ້ງຄ່າ Serilog ທັງໃນລະຫວ່າງການ booting ແລະໃນລະຫວ່າງການບັນທຶກ HTTP Request
/// </summary>
/// <remarks>
/// ລະບົບບໍ່ໄດ້ບັນທຶກ Request Body ຫຼື Response Body ລົງ Log ເລີຍ
/// ເນື່ອງຈາກວ່າເສັ້ນທາງເຊັ່ນ login ແລະ reset-password ມີລະຫັດຜ່ານແລະ token ໃນເນື້ອຫາ
/// </remarks>
public static class SerilogConfiguration
{
    /// <summary>
    /// ສ້າງ Logger ຊົ່ວຄາວກ່ອນອ່ານ configuration
    /// ໃຊ້ເພື່ອໃຫ້ຂໍ້ຜິດພາດທີ່ເກີດຂື້ນໃນລະຫວ່າງການຕັ້ງກໍ່ຖືກບັນທຶກ
    /// </summary>
    public static void CreateBootstrapLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .CreateBootstrapLogger();
    }

    /// <summary>ຜູກ Serilog ເຝາະກັບ Host ແລະອ່ານມູນຄ່າຈາກ configuration section ຊື່ Serilog</summary>
    /// <param name="builder">ຕົວສ້າງແອັບພລິເຄຊັນ</param>
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
    /// ສະຫຼຸບໜຶ່ງແຖວຕໍ່ຄຳຮ້ອງຂໍ. ກັບຂໍ້ມູນບໍລິບົດທີ່ສາມາດຖືກນໍາໃຊ້ເພື່ອຄົ້ນຫາຄືນ
    /// </summary>
    /// <param name="app">ທໍ່ຄໍາຮ້ອງສະຝັກ</param>
    public static IApplicationBuilder UseDormFlowRequestLogging(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        return app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "{RequestMethod} {RequestPath} ຕອບກັບ {StatusCode} ໃຊ້ເວລາ {Elapsed:0.0000} ms";

            options.GetLevel = GetLogLevel;
            options.EnrichDiagnosticContext = EnrichDiagnosticContext;
        });
    }

    /// <summary>
    /// ບໍ່ໄດ້ບັນທຶກການໂທ Health Check ທີ່ຜ່ານໄປຢ່າງຄ່ອງແຄ້ວເປັນລະດັບ Information
    /// ເນື່ອງຈາກວ່າ orchestrator ໂທມາເລື້ອຍໆຈົນຈົມນໍ້າ Log ທີ່ມີຄວາມຝາຍທີ່ແທ້ຈິງ
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
