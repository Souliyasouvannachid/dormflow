using System.Net;
using System.Text.Json;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>ກວດເບິ່ງວ່າເອກະສານ OpenAPI ສ້າງແລະປະກາດ JWT Bearer ຮັກສາມັນເປັນຄວາມຈິງ</summary>
[Collection(ApiTestFixtures.Name)]
public class OpenApiDocumentTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Theory]
    [InlineData(null, HttpStatusCode.NotFound)]
    [InlineData(false, HttpStatusCode.NotFound)]
    [InlineData(true, HttpStatusCode.OK)]
    public async Task Production_DocumentationRequiresExplicitOptIn(bool? enabled, HttpStatusCode expected)
    {
        using var production = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Production");
            builder.UseSetting("Email:Provider", "Disabled");
            builder.UseSetting("BackgroundJobs:Enabled", "false");
            builder.UseSetting("Database:AutoMigrate", "false");
            builder.UseSetting("Database:FailOnPendingMigrations", "false");
            builder.UseSetting("Database:SeedOnStartup", "false");
            if (enabled.HasValue)
            {
                builder.UseSetting("Api:EnableDocumentation", enabled.Value.ToString());
            }
        });
        using HttpClient client = production.CreateClient();

        using HttpResponseMessage schema = await client.GetAsync(
            new Uri("/openapi/v1.json", UriKind.Relative), TestContext.Current.CancellationToken);
        using HttpResponseMessage reference = await client.GetAsync(
            new Uri("/scalar/v1", UriKind.Relative), TestContext.Current.CancellationToken);

        schema.StatusCode.Should().Be(expected);
        reference.StatusCode.Should().Be(expected);

        // Publishing documentation must not make application data anonymous.
        using HttpResponseMessage rooms = await client.GetAsync(
            new Uri("/api/v1/rooms", UriKind.Relative), TestContext.Current.CancellationToken);
        rooms.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

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
