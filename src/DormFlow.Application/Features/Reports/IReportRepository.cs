using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;

namespace DormFlow.Application.Features.Reports;

/// <summary>ການກັ່ນຕອງບົດລາຍງານສະຫຼຸບລາຍຮັບ</summary>
public sealed record RevenueReportFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    DateOnly PeriodFrom,
    DateOnly PeriodTo,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ສະຫຼຸບຫຍໍ້ຂອງບົດລາຍງານລາຍໄດ້ກ່ອນທີ່ຈະປ່ຽນເປັນ Response</summary>
public sealed record RevenueSummary(
    int InvoiceCount,
    int PaidInvoiceCount,
    int OverdueInvoiceCount,
    decimal TotalInvoiced,
    decimal TotalCollected);

/// <summary>ຍອດເກັບໄດ້ລວມຕາມເດືອນ</summary>
public sealed record RevenueTrendSummary(DateOnly PeriodMonth, decimal TotalCollected);

/// <summary>ການກັ່ນຕອງລາຍງານໃບແຈ້ງໜີ້ທີ່ບໍ່ໄດ້ຈ່າຍ</summary>
public sealed record OutstandingInvoicesFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ແຖວໜຶ່ງຂອງຂໍ້ມູນດິບຂອງລາຍງານໃບແຈ້ງໜີ້ທີ່ບໍ່ໄດ້ຮັບຄ່າຈ້າງ. ກ່ອນທີ່ຈະຄິດໄລ່ຍອດເງິນ/ຈຳນວນມື້ທີ່ເກີນກຳນົດ</summary>
public sealed record OutstandingInvoiceRow(
    Guid InvoiceId,
    string InvoiceNumber,
    Guid ContractId,
    Guid? DormitoryId,
    DateOnly DueDate,
    decimal TotalAmount,
    decimal PaidAmount);

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນເພື່ອສ້າງບົດລາຍງານໂດຍການລວມຂໍ້ມູນຈາກຫຼາຍໆຄົນ Entity</summary>
public interface IReportRepository
{
    /// <summary>ສະຫຼຸບລາຍຮັບຕາມຕົວກອງທີ່ກໍານົດໄວ້</summary>
    /// <param name="filter">ການກັ່ນຕອງ</param>
    /// <param name="asOfDate">ວັນທີອ້າງອີງແມ່ນໃຊ້ເພື່ອຄິດໄລ່ຈຳນວນໃບເກັບເງິນທີ່ໝົດກຳນົດ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task<RevenueSummary> GetRevenueSummaryAsync(
        RevenueReportFilter filter,
        DateOnly asOfDate,
        CancellationToken cancellationToken = default);

    /// <summary>ດຶງຍອດເກັບໄດ້ລວມຫຼາຍເດືອນດ້ວຍ query ດຽວ</summary>
    Task<IReadOnlyList<RevenueTrendSummary>> GetRevenueTrendAsync(
        RevenueReportFilter filter,
        CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາລາຍຊື່ໃບແຈ້ງໜີ້ທີ່ຄ້າງໄວ້. ຈັດຮຽງຕາມວັນທີຄົບກຳນົດການຈ່າຍເງິນ (ໃກ້ທີ່ສຸດກັບວັນທີຄົບກຳນົດກ່ອນ)</summary>
    Task<PagedResult<OutstandingInvoiceRow>> GetOutstandingInvoicesAsync(
        OutstandingInvoicesFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ດຶງເອົາບັນຊີລາຍຊື່ຂອງໃບແຈ້ງໜີ້ທີ່ຍັງຄ້າງຄາທັງໝົດໂດຍອີງໃສ່ຕົວກອງ, ບໍ່ໄດ້ໃສ່ໜ້າ, ໃຊ້ສຳລັບການສົ່ງອອກໄຟລ໌
    /// ຈຳກັດຈຳນວນແຖວສູງສຸດເປັນລະດັບໜຶ່ງ Infrastructure ນີ້ປ້ອງກັນການດຶງຂໍ້ມູນຫຼາຍເກີນໄປໃນຄໍາຮ້ອງຂໍດຽວ
    /// </summary>
    Task<IReadOnlyList<OutstandingInvoiceRow>> GetOutstandingInvoicesForExportAsync(
        OutstandingInvoicesFilter filter,
        CancellationToken cancellationToken = default);
}
