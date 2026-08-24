using System.Text;
using DormFlow.Api.Authorization;
using DormFlow.Contracts.Common;
using DormFlow.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace DormFlow.Api.Configuration;

/// <summary>ตั้งค่าการยืนยันตัวตนด้วย JWT Bearer และการตรวจสิทธิ์แบบไดนามิก</summary>
public static class AuthenticationConfiguration
{
    /// <summary>ลงทะเบียน JWT Bearer Authentication และ Permission-Based Authorization</summary>
    /// <param name="services">Service collection ของแอปพลิเคชัน</param>
    /// <param name="configuration">Configuration ของแอปพลิเคชัน</param>
    public static IServiceCollection AddDormFlowAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        JwtOptions jwtOptions = new();
        configuration.GetSection(JwtOptions.SectionName).Bind(jwtOptions);

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(jwtOptions.ClockSkewSeconds),
                };

                // เขียน Response รูปแบบเดียวกับส่วนอื่นของระบบเอง แทนที่จะปล่อยให้ Global Exception Middleware จัดการ
                // เพราะ JwtBearer เขียน Response ไปแล้วบางส่วนก่อนที่ Middleware กลางจะไปถึง
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        return WriteUnauthorizedResponseAsync(context.Response);
                    },
                    OnForbidden = context => WriteForbiddenResponseAsync(context.Response),
                };
            });

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddAuthorization();

        return services;
    }

    private static Task WriteUnauthorizedResponseAsync(HttpResponse response)
    {
        response.StatusCode = StatusCodes.Status401Unauthorized;
        response.ContentType = "application/json; charset=utf-8";

        return response.WriteAsJsonAsync(
            ApiResponse.Fail(ApiMessages.Unauthorized, response.HttpContext.TraceIdentifier),
            DormFlowJsonSerializerOptions.Default);
    }

    private static Task WriteForbiddenResponseAsync(HttpResponse response)
    {
        response.StatusCode = StatusCodes.Status403Forbidden;
        response.ContentType = "application/json; charset=utf-8";

        return response.WriteAsJsonAsync(
            ApiResponse.Fail(ApiMessages.Forbidden, response.HttpContext.TraceIdentifier),
            DormFlowJsonSerializerOptions.Default);
    }
}
