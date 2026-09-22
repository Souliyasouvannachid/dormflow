using System.Net;
using System.Net.Http.Json;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.AuditLogs;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Organizations;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>ການທົດສອບການອ່ານ Audit Log ຜ່ານ HTTP ຄວາມຈິງ</summary>
[Collection(ApiTestFixtures.Name)]
public class AuditLogsEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task ການສ້າງອົງກອນ_ຕ້ອງໄດ້ຮັບການບັນທຶກໄວ້_Audit_Log_ແລະຊອກຫາມັນໂດຍຜ່ານ_Endpoint()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        string code = $"ORG{Guid.NewGuid():N}";

        using HttpResponseMessage createResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/organizations", UriKind.Relative),
            new CreateOrganizationRequest { Code = code, Name = $"ອົງກອນການທົດສອບ {code}" },
            TestContext.Current.CancellationToken);

        ApiResponse<OrganizationResponse>? createdBody = await createResponse.Content
            .ReadFromJsonAsync<ApiResponse<OrganizationResponse>>(TestContext.Current.CancellationToken);
        OrganizationResponse organization = createdBody!.Data!;

        using HttpResponseMessage auditResponse = await client.GetAsync(
            new Uri(
                $"/api/v1/audit-logs?organizationId={organization.Id}&action=Create&entityName=Organization",
                UriKind.Relative),
            TestContext.Current.CancellationToken);

        auditResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        ApiResponse<PagedResult<AuditLogResponse>>? auditBody = await auditResponse.Content
            .ReadFromJsonAsync<ApiResponse<PagedResult<AuditLogResponse>>>(TestContext.Current.CancellationToken);

        auditBody!.Data!.Items.Should().ContainSingle(log =>
            log.EntityId == organization.Id.ToString() && log.Action == "Create");
    }

    [Fact]
    public async Task ຜູ້ໃຊ້ທີ່ບໍ່ມີສິດພິເສດ_audit_logs_view_ຕ້ອງໄດ້ຮັບ_403()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.Staff);

        using HttpResponseMessage response = await client.GetAsync(
            new Uri("/api/v1/audit-logs", UriKind.Relative),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
