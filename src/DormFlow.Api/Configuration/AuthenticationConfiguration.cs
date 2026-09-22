using System.Text;
using DormFlow.Api.Authorization;
using DormFlow.Contracts.Common;
using DormFlow.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace DormFlow.Api.Configuration;

/// <summary>ຕັ້ງຄ່າການພິສູດຢືນຢັນເຊັ່ນກັນ JWT Bearer ແລະການຢືນຢັນແບບເຄື່ອນໄຫວ</summary>
public static class AuthenticationConfiguration
{
    /// <summary>ລົງທະບຽນ JWT Bearer Authentication ແລະ Permission-Based Authorization</summary>
    /// <param name="services">Service collection ຂອງຄໍາຮ້ອງສະຝັກ</param>
    /// <param name="configuration">Configuration ຂອງຄໍາຮ້ອງສະຝັກ</param>
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

                // ຂຽນ Response ຮູບແບບແມ່ນຄືກັນກັບສ່ວນທີ່ເຫຼືອຂອງລະບົບຕົວມັນເອງ. ແທນທີ່ຈະປ່ອຍໃຫ້ Global Exception Middleware ຈັດການ
                // ເນື່ອງຈາກວ່າ JwtBearer ຂຽນ Response ໄປກ່ອນ Middleware ກາງຈະໄປຮອດ
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
