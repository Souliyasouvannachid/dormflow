using DormFlow.Api.OpenApi;
using DormFlow.Api.Options;
using Scalar.AspNetCore;

namespace DormFlow.Api.Configuration;

/// <summary>ຕັ້ງຄ່າເອກະສານ OpenAPI ແລະໜ້າເອກະສານແບບໂຕ້ຕອບ</summary>
public static class OpenApiConfiguration
{
    /// <summary>ຊື່ເອກະສານຂອງ API ຮຸ່ນ 1</summary>
    public const string V1DocumentName = "v1";

    /// <summary>ລົງທະບຽນການສ້າງເອກະສານ OpenAPI</summary>
    /// <param name="services">Service collection ຂອງຄໍາຮ້ອງສະຝັກ</param>
    public static IServiceCollection AddDormFlowOpenApi(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // ໂທ services.AddOpenApi(...) ໂດຍກົງທີ່ຈໍາເປັນເພື່ອດຶງ XML Comments ໃສ່ເອກະສານ
        // ເນື່ອງຈາກວ່າ source generator ຂອງ .NET 10 ເຮັດວຽກໂດຍ intercept ໂທຫານີ້ໃນລະຫັດຄໍາຮ້ອງສະຝັກຕົວມັນເອງ
        services.AddOpenApi(V1DocumentName, options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        return services;
    }

    /// <summary>
    /// ເປີດເສັ້ນທາງເອກະສານ OpenAPI ແລະໜ້າເອກະສານແບບໂຕ້ຕອບ
    /// </summary>
    /// <param name="app">ແອັບພລິເຄຊັນທີ່ສ້າງຂຶ້ນ</param>
    /// <remarks>
    /// ສະພາບແວດລ້ອມເປີດເທົ່ານັ້ນທີ່ບໍ່ມີ Production
    /// ເພື່ອເປີດໃນ Production ປົກປ້ອງຕົນເອງສະເໝີໂດຍການຢັ້ງຢືນຕົວຕົນຂອງທ່ານກ່ອນ
    /// </remarks>
    public static WebApplication UseDormFlowOpenApi(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        if (app.Environment.IsProduction())
        {
            return app;
        }

        app.MapGet("/", () => Results.Redirect("/scalar/")).AllowAnonymous();
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
