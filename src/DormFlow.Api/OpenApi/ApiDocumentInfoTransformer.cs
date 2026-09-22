using DormFlow.Api.Options;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace DormFlow.Api.OpenApi;

/// <summary>
/// ກໍານົດຊື່ລະບົບຂອງເອກະສານ, ຄໍາອະທິບາຍ, ແລະການຕິດຕໍ່ OpenAPI ຈາກມູນຄ່າໃນ configuration
/// </summary>
/// <remarks>
/// ຕ້ອງລົງທະບຽນຜ່ານ AddApiVersioning().AddOpenApi(...) ເທົ່ານັ້ນ
/// ເນື່ອງຈາກວ່າ transformer ຂອງ Asp.Versioning ຂຽນທັບ Info.Title ໂດຍຊື່ assembly
/// ການລົງທະບຽນຢູ່ໃນຈຸດນັ້ນເຮັດໃຫ້ transformer ອັນນີ້ເຮັດວຽກຕໍ່ມາແລະຕົວຈິງແລ້ວໄດ້ຮັບຄ່າທີ່ກໍານົດໄວ້
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
