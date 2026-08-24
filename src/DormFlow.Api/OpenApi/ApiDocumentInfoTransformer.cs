using DormFlow.Api.Options;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace DormFlow.Api.OpenApi;

/// <summary>
/// กำหนดชื่อระบบ คำอธิบาย และผู้ติดต่อของเอกสาร OpenAPI จากค่าใน configuration
/// </summary>
/// <remarks>
/// ต้องลงทะเบียนผ่าน AddApiVersioning().AddOpenApi(...) เท่านั้น
/// เพราะ transformer ของ Asp.Versioning เขียนทับ Info.Title ด้วยชื่อ assembly
/// การลงทะเบียนที่จุดนั้นทำให้ transformer ตัวนี้ทำงานทีหลังและได้ค่าตามที่ตั้งไว้จริง
/// </remarks>
public sealed class ApiDocumentInfoTransformer(IOptions<ApiOptions> apiOptions) : IOpenApiDocumentTransformer
{
    private readonly ApiOptions _apiOptions = apiOptions.Value;

    /// <inheritdoc />
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);

        document.Info ??= new OpenApiInfo();
        document.Info.Title = _apiOptions.Title;
        document.Info.Description = _apiOptions.Description;

        if (!string.IsNullOrWhiteSpace(_apiOptions.ContactName) || !string.IsNullOrWhiteSpace(_apiOptions.ContactEmail))
        {
            document.Info.Contact = new OpenApiContact
            {
                Name = _apiOptions.ContactName,
                Email = _apiOptions.ContactEmail,
            };
        }

        return Task.CompletedTask;
    }
}
