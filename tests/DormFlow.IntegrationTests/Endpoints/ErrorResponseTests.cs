using System.Net;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>ตรวจพฤติกรรมของไปป์ไลน์เมื่อเรียกเส้นทางที่ไม่มีอยู่จริง</summary>
[Collection(ApiTestFixtures.Name)]
public class ErrorResponseTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task เส้นทางที่ไม่มีอยู่จริง_ต้องได้_404()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(
            new Uri("/api/v1/ไม่มีเส้นทางนี้", UriKind.Relative),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task คำขอที่ไม่ได้ระบุ_Origin_ต้องผ่านได้ตามปกติ()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(
            new Uri("/health/live", UriKind.Relative),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
