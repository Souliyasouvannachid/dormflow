using DormFlow.Contracts.Common;
using DormFlow.Contracts.Dashboard;
using DormFlow.Contracts.Reports;

namespace DormFlow.Application.Features.Reports;

/// <summary>Use Case ທັງຝົດຂອງໂມດູນບົດລາຍງານ</summary>
public interface IReportService
{
    /// <summary>ສະຫຼຸບລາຍຮັບສໍາລັບໄລຍະເວລາທີ່ກໍານົດໄວ້</summary>
    Task<RevenueReportResponse> GetRevenueSummaryAsync(
        RevenueReportRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ດຶງ trend ລາຍຮັບຫຼາຍເດືອນໃນຄຳຮ້ອງຂໍດຽວ</summary>
    Task<IReadOnlyList<RevenueTrendPointResponse>> GetRevenueTrendAsync(
        DateOnly periodFrom,
        DateOnly periodTo,
        CancellationToken cancellationToken = default);

    /// <summary>ສົ່ງອອກບົດລາຍງານສະຫຼຸບລາຍຮັບເພື່ອຍື່ນ CSV</summary>
    Task<byte[]> ExportRevenueSummaryAsync(
        RevenueReportRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາລາຍຊື່ໃບແຈ້ງໜີ້ທີ່ຄ້າງໄວ້</summary>
    Task<PagedResult<OutstandingInvoiceItemResponse>> GetOutstandingInvoicesAsync(
        OutstandingInvoicesReportRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ສົ່ງອອກບົດລາຍງານໃບແຈ້ງໜີ້ທີ່ຍັງຄ້າງຄາເປັນໄຟລ໌ CSV</summary>
    Task<byte[]> ExportOutstandingInvoicesAsync(
        OutstandingInvoicesReportRequest request,
        CancellationToken cancellationToken = default);
}
