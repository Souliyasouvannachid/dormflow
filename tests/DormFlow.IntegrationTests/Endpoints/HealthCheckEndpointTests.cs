using System.Net;
using System.Text.Json;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>
/// ກວດເບິ່ງວ່າເສັ້ນທາງນັ້ນ Health Check ສໍາເລັດວຽກງານຕາມທີ່ໄດ້ກໍານົດໄວ້ orchestrator ແລະ docker compose ຕ້ອງໃຊ້
/// </summary>
[Collection(ApiTestFixtures.Name)]
public class HealthCheckEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task Health_live_ຕ້ອງຕອບວ່າລະບົບຍັງເຮັດວຽກຢູ່()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(new Uri("/health/live", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonElement payload = await ReadJsonAsync(response);
        payload.GetProperty("status").GetString().Should().Be("Healthy");
    }

    [Fact]
    public async Task Health_live_ເຈົ້າບໍ່ຄວນກວດເບິ່ງຖານຂໍ້ມູນ()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(new Uri("/health/live", UriKind.Relative), TestContext.Current.CancellationToken);

        JsonElement payload = await ReadJsonAsync(response);
        string[] checkNames = [.. payload.GetProperty("checks").EnumerateArray().Select(check => check.GetProperty("name").GetString()!)];

        checkNames.Should().ContainSingle().Which.Should().Be("self");
    }

    [Fact]
    public async Task Health_ready_ຕ້ອງກວດສອບການເຊື່ອມຕໍ່_PostgreSQL()
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
    public async Task Health_ຜົນການທົດສອບທັງໝົດຕ້ອງຖືກລວມເຂົ້າ()
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
