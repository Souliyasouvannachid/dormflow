using System.Net;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>ກວດເບິ່ງພຶດຕິກໍາຂອງທໍ່ໃນເວລາທີ່ໂທຫາເສັ້ນທາງທີ່ບໍ່ມີຢູ່</summary>
[Collection(ApiTestFixtures.Name)]
public class ErrorResponseTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task ເສັ້ນທາງທີ່ບໍ່ມີຢູ່ແທ້ໆ_ຕ້ອງໄດ້ຮັບ_404()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(
            new Uri("/api/v1/ບໍ່ມີເສັ້ນທາງນີ້", UriKind.Relative),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ຄຳຮ້ອງຂໍທີ່ບໍ່ລະບຸ_Origin_ຕ້ອງສາມາດຜ່ານໄດ້ຕາມປົກກະຕິ()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(
            new Uri("/health/live", UriKind.Relative),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
