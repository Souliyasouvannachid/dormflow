using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace DormFlow.Api.OpenApi;

/// <summary>
/// ເພີ່ມ JWT Bearer Security Scheme ເຂົ້າໄປໃນເອກະສານ OpenAPI
/// </summary>
/// <remarks>
/// ປະກາດໃນລະດັບເອກະສານພຽງແຕ່ຄັ້ງດຽວ. ສໍາລັບຜູ້ໃຊ້ເອກະສານທີ່ຈະກົດ Authorize ຫຼັງຈາກນັ້ນ, ທ່ານສາມາດໂທຫາທຸກບ່ອນ Endpoint
/// Endpoint ທີ່ບໍ່ຮຽກຮ້ອງໃຫ້ມີການຢັ້ງຢືນຕົວຕົນຍັງຖືກຝາຍ AllowAnonymous ໃນລະດັບ Endpoint ແຍກ
/// </remarks>
public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    /// <summary>ຊື່ Security Scheme ໃຊ້ເປັນເອກະສານອ້າງອີງໃນເອກະສານ</summary>
    public const string SchemeName = "Bearer";

    /// <inheritdoc />
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>(StringComparer.Ordinal);
        document.Components.SecuritySchemes[SchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "ໃສ່ Access Token ທີ່ໄດ້ຈາກ POST /api/v1/auth/login ໂດຍບໍ່ຕ້ອງພິມຄຳວ່າ Bearer ນຳໜ້າ",
        };

        return Task.CompletedTask;
    }
}
