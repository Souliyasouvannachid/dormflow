using DormFlow.Api.Options;
using Microsoft.Extensions.Options;

namespace DormFlow.Api.Configuration;

/// <summary>ຕັ້ງຄ່າ CORS ຈາກ configuration ເທົ່ານັ້ນ</summary>
public static class CorsConfiguration
{
    /// <summary>
    /// ລົງທະບຽນ CORS policy ອີງຕາມບັນຊີລາຍຊື່ Origin ລະບຸໄວ້ໃນ configuration
    /// </summary>
    /// <param name="services">Service collection ຂອງຄໍາຮ້ອງສະຝັກ</param>
    /// <param name="configuration">Configuration ຂອງຄໍາຮ້ອງສະຝັກ</param>
    /// <param name="environment">ສະພາບແວດລ້ອມແລ່ນ</param>
    /// <exception cref="InvalidOperationException">
    /// ເມື່ອແລ່ນເຂົ້າ Production ແຕ່ບໍ່ໄດ້ລະບຸ Origin ວ່າອະນຸຍາດ
    /// ລະບົບຈະບໍ່ເລີ່ມຕົ້ນ. ແທນທີ່ຈະເປີດມັນຂຶ້ນກັບທຸກຄົນໂດຍບັງເອີນ Origin
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

        if (allowedOrigins.Length == 0)
        {
            throw new InvalidOperationException(
                "ບໍ່ພົບລາຍການ Cors:AllowedOrigins ສຳລັບສະພາບແວດລ້ອມ " +
                $"{environment.EnvironmentName} " +
                "ກຳນົດຜ່ານ environment variable ຊື່ Cors__AllowedOrigins__0 ເປັນຕົ້ນໄປ");
        }

        services.AddCors(options => options.AddPolicy(CorsOptions.PolicyName, policy =>
        {
            policy.WithOrigins(allowedOrigins);

            policy.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithExposedHeaders("X-Trace-Id");
        }));

        return services;
    }

    /// <summary>ອ່ານລາຍການ Origin ອະນຸຍາດຫຼັງຈາກການລົງທະບຽນ, ໃຊ້ໃນເວລາບັນທຶກ Log ເມື່ອເລີ່ມຕົ້ນ</summary>
    /// <param name="services">Service provider ຂອງຄໍາຮ້ອງສະຝັກ</param>
    public static IReadOnlyList<string> GetAllowedOrigins(this IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return [.. services.GetRequiredService<IOptions<CorsOptions>>().Value.AllowedOrigins];
    }
}
