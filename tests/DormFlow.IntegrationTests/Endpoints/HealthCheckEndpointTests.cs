using System.Net;
using System.Text.Json;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>
/// ตรวจว่าเส้นทาง Health Check ทำงานครบตามที่ orchestrator และ docker compose ต้องใช้
/// </summary>
[Collection(ApiTestFixtures.Name)]
public class HealthCheckEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task Health_live_ต้องตอบว่าระบบยังทำงานอยู่()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(new Uri("/health/live", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonElement payload = await ReadJsonAsync(response);
        payload.GetProperty("status").GetString().Should().Be("Healthy");
    }

    [Fact]
    public async Task Health_live_ต้องไม่ตรวจฐานข้อมูล()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(new Uri("/health/live", UriKind.Relative), TestContext.Current.CancellationToken);

        JsonElement payload = await ReadJsonAsync(response);
        string[] checkNames = [.. payload.GetProperty("checks").EnumerateArray().Select(check => check.GetProperty("name").GetString()!)];

        checkNames.Should().ContainSingle().Which.Should().Be("self");
    }

    [Fact]
    public async Task Health_ready_ต้องตรวจการเชื่อมต่อ_PostgreSQL()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(new Uri("/health/ready", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonElement payload = await ReadJsonAsync(response);
        payload.GetProperty("status").GetString().Should().Be("Healthy");

        string[] checkNames = [.. payload.GetProperty("checks").EnumerateArray().Select(check => check.GetProperty("name").GetString()!)];
        checkNames.Should().ContainSingle().Which.Should().Be("postgresql");
    }

    [Fact]
    public async Task Health_ต้องรวมผลการตรวจทุกรายการ()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(new Uri("/health", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonElement payload = await ReadJsonAsync(response);
        string[] checkNames = [.. payload.GetProperty("checks").EnumerateArray().Select(check => check.GetProperty("name").GetString()!)];

        checkNames.Should().BeEquivalentTo(["self", "postgresql"]);
    }

    private static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response)
    {
        string body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        using JsonDocument document = JsonDocument.Parse(body);
        return document.RootElement.Clone();
    }
}
