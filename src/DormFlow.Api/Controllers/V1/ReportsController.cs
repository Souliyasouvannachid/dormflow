using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Reports;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Reports;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint ສໍາລັບການລາຍງານ</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reports")]
[Produces("application/json")]
public sealed class ReportsController(IReportService reportService) : ControllerBase
{
    private readonly IReportService _reportService = reportService;

    /// <summary>ສະຫຼຸບລາຍຮັບສໍາລັບໄລຍະເວລາທີ່ກໍານົດໄວ້</summary>
    [HttpGet("revenue")]
    [RequirePermission(PermissionCodes.Reports.View)]
    [ProducesResponseType(typeof(ApiResponse<RevenueReportResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<RevenueReportResponse>>> GetRevenueSummary(
        [FromQuery] RevenueReportRequest request,
        CancellationToken cancellationToken)
    {
        RevenueReportResponse result = await _reportService.GetRevenueSummaryAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>ສົ່ງອອກບົດລາຍງານສະຫຼຸບລາຍຮັບເພື່ອຍື່ນ CSV</summary>
    [HttpGet("revenue/export")]
    [RequirePermission(PermissionCodes.Reports.Export)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExportRevenueSummary(
        [FromQuery] RevenueReportRequest request,
        CancellationToken cancellationToken)
    {
        byte[] csv = await _reportService.ExportRevenueSummaryAsync(request, cancellationToken);
        return File(csv, "text/csv", "revenue-report.csv");
    }

    /// <summary>ດຶງເອົາລາຍຊື່ໃບແຈ້ງໜີ້ທີ່ຄ້າງໄວ້</summary>
    [HttpGet("outstanding-invoices")]
    [RequirePermission(PermissionCodes.Reports.View)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<OutstandingInvoiceItemResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<OutstandingInvoiceItemResponse>>>> GetOutstandingInvoices(
        [FromQuery] OutstandingInvoicesReportRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<OutstandingInvoiceItemResponse> result =
            await _reportService.GetOutstandingInvoicesAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>ສົ່ງອອກບົດລາຍງານໃບແຈ້ງໜີ້ທີ່ຍັງຄ້າງຄາເປັນໄຟລ໌ CSV</summary>
    [HttpGet("outstanding-invoices/export")]
    [RequirePermission(PermissionCodes.Reports.Export)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportOutstandingInvoices(
        [FromQuery] OutstandingInvoicesReportRequest request,
        CancellationToken cancellationToken)
    {
        byte[] csv = await _reportService.ExportOutstandingInvoicesAsync(request, cancellationToken);
        return File(csv, "text/csv", "outstanding-invoices.csv");
    }
}
