using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace DormFlow.Api.OpenApi;

/// <summary>
/// เพิ่ม JWT Bearer Security Scheme ลงในเอกสาร OpenAPI
/// </summary>
/// <remarks>
/// ประกาศไว้ที่ระดับเอกสารครั้งเดียว เพื่อให้ผู้ใช้เอกสารกด Authorize แล้วเรียกได้ทุก Endpoint
/// Endpoint ที่ไม่ต้องยืนยันตัวตนถูกกำกับด้วย AllowAnonymous ที่ระดับ Endpoint แยกต่างหาก
/// </remarks>
public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    /// <summary>ชื่อ Security Scheme ที่ใช้อ้างอิงในเอกสาร</summary>
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
            Description = "ใส่ Access Token ที่ได้จาก POST /api/v1/auth/login โดยไม่ต้องพิมพ์คำว่า Bearer นำหน้า",
        };

        return Task.CompletedTask;
    }
}
