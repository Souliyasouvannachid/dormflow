using System.Net;
using System.Text.Json;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>ກວດເບິ່ງວ່າເອກະສານ OpenAPI ສ້າງແລະປະກາດ JWT Bearer ຮັກສາມັນເປັນຄວາມຈິງ</summary>
[Collection(ApiTestFixtures.Name)]
public class OpenApiDocumentTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task ເອກະສານ_OpenAPI_ຮຸ່ນ_v1_ຕ້ອງສາມາດເຂົ້າເຖິງໄດ້()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(new Uri("/openapi/v1.json", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ເອກະສານ_OpenAPI_ຕ້ອງປະກາດ_Bearer_Security_Scheme()
    {
        using HttpClient client = _factory.CreateClient();

        string body = await client.GetStringAsync(new Uri("/openapi/v1.json", UriKind.Relative), TestContext.Current.CancellationToken);

        using JsonDocument document = JsonDocument.Parse(body);
        JsonElement bearer = document.RootElement
            .GetProperty("components")
            .GetProperty("securitySchemes")
            .GetProperty("Bearer");

        bearer.GetProperty("type").GetString().Should().Be("http");
        bearer.GetProperty("scheme").GetString().Should().Be("bearer");
        bearer.GetProperty("bearerFormat").GetString().Should().Be("JWT");
    }

    [Fact]
    public async Task ເອກະສານ_OpenAPI_ຊື່ລະບົບຕ້ອງຖືກນໍາໃຊ້ຈາກ_Configuration()
    {
        using HttpClient client = _factory.CreateClient();

        string body = await client.GetStringAsync(new Uri("/openapi/v1.json", UriKind.Relative), TestContext.Current.CancellationToken);

        using JsonDocument document = JsonDocument.Parse(body);
        document.RootElement.GetProperty("info").GetProperty("title").GetString()
            .Should().Be("DormFlow API ພາສາລາວ");
    }
}
