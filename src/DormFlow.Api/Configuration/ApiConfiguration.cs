using System.Diagnostics;
using System.Globalization;
using Asp.Versioning;
using DormFlow.Api.OpenApi;
using DormFlow.Api.Options;
using DormFlow.Api.Services;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DormFlow.Api.Configuration;

/// <summary>ตั้งค่าส่วนกลางของชั้น API เช่น Controller, API Versioning และรูปแบบ Response</summary>
public static class ApiConfiguration
{
    /// <summary>ลงทะเบียนบริการพื้นฐานของชั้น API</summary>
    /// <param name="services">Service collection ของแอปพลิเคชัน</param>
    /// <param name="configuration">Configuration ของแอปพลิเคชัน</param>
    public static IServiceCollection AddDormFlowApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<ApiOptions>()
            .Bind(configuration.GetSection(ApiOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddControllers()
            .AddJsonOptions(options => DormFlowJsonSerializerOptions.Configure(options.JsonSerializerOptions));

        ConfigureInvalidModelStateResponse(services);
        AddApiVersioning(services);

        return services;
    }

    /// <summary>
    /// แปลงข้อผิดพลาดจาก Model Binding ให้อยู่ในรูปแบบ ApiResponse เดียวกับข้อผิดพลาดอื่น
    /// ไม่เช่นนั้น ASP.NET Core จะตอบเป็น ProblemDetails ซึ่งคนละรูปแบบกับที่เหลือทั้งระบบ
    /// </summary>
    private static void ConfigureInvalidModelStateResponse(IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                Dictionary<string, string[]> errors = context.ModelState
                    .Where(entry => entry.Value?.Errors.Count > 0)
                    .ToDictionary(
                        entry => ToCamelCase(entry.Key),
                        entry => entry.Value!.Errors
                            .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                                ? "ค่าที่ระบุไม่ถูกต้อง"
                                : error.ErrorMessage)
                            .ToArray(),
                        StringComparer.Ordinal);

                string traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;

                return new BadRequestObjectResult(
                    ApiResponse.ValidationFailed(ApiMessages.ValidationFailed, errors, traceId));
            };
        });
    }

    private static void AddApiVersioning(IServiceCollection services)
    {
        services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            })
            .AddOpenApi(options => options.Document.AddDocumentTransformer<ApiDocumentInfoTransformer>());
    }

    private static string ToCamelCase(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        string[] segments = value.Split('.');
        for (int i = 0; i < segments.Length; i++)
        {
            string segment = segments[i];
            if (segment.Length > 0 && char.IsUpper(segment[0]))
            {
                segments[i] = string.Create(CultureInfo.InvariantCulture, $"{char.ToLowerInvariant(segment[0])}{segment[1..]}");
            }
        }

        return string.Join('.', segments);
    }
}
