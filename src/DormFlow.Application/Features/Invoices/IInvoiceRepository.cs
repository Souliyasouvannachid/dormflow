using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Invoices;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງບັນຊີລາຍຊື່ paginated ຂອງໃບແຈ້ງຜີ້</summary>
public sealed record InvoiceListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? ContractId,
    InvoiceStatus? Status,
    DateOnly? PeriodMonth,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນໃບແຈ້ງໜີ້ໃນຖານຂໍ້ມູນ</summary>
public interface IInvoiceRepository
{
    /// <summary>ອ່ານໃບແຈ້ງໜີ້ຈາກລະຫັດອ້າງອີງ</summary>
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ກວດເບິ່ງວ່າມີໃບແຈ້ງໜີ້ທີ່ໃຊ້ເບີນີ້ຢູ່ໃນອົງກອນດຽວກັນຫຼືບໍ່</summary>
    Task<bool> ExistsByInvoiceNumberAsync(
        Guid organizationId,
        string invoiceNumber,
        CancellationToken cancellationToken = default);

    /// <summary>ກວດເບິ່ງວ່າສັນຍາການເຊົ່ານີ້ໄດ້ຖືກ invoice ສໍາລັບເດືອນທີ່ລະບຸໄວ້ຫຼືບໍ່</summary>
    Task<bool> ExistsForContractPeriodAsync(
        Guid contractId,
        DateOnly periodMonth,
        CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາລາຍຊື່ໃບເກັບເງິນທີ່ຕັ້ງໄວ້ຕາມຕົວກອງທີ່ລະບຸໄວ້</summary>
    Task<PagedResult<Invoice>> GetPagedAsync(
        InvoiceListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ດຶງເອົາລາຍຊື່ໃບແຈ້ງໜີ້ທີ່ຍັງບໍ່ທັນໄດ້ໝາຍວ່າໝົດກຳນົດ (Issued/PartiallyPaid)
    /// ແຕ່ວັນທີ່ກໍານົດການຊໍາລະໄດ້ຜ່ານໄປກ່ອນວັນທີທີ່ກໍານົດໄວ້
    /// ກາງຄືນ Entity ປ່ຽນແບບຟອມການຕິດຕາມ (ບໍ່ AsNoTracking) ເນື່ອງຈາກວ່າມັນຖືກເອີ້ນວ່າສືບຕໍ່ແກ້ໄຂສະຖານະການທັນທີ
    /// ໃຊ້ໂດຍ Background Job ເທົ່ານັ້ນ, ບໍ່ໄດ້ຖືກກັ່ນຕອງໂດຍອົງກອນ/ຫໍພັກເພາະວ່າພວກເຂົາເຮັດວຽກທົ່ວລະບົບທັງຝົດ
    /// </summary>
    Task<IReadOnlyList<Invoice>> GetOverdueCandidatesAsync(
        DateOnly asOfDate,
        int batchSize,
        CancellationToken cancellationToken = default);

    /// <summary>ເພີ່ມໃບແຈ້ງໜີ້ໃໝ່</summary>
    Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);

    /// <summary>ບັນທຶກການປ່ຽນແປງທັງໝົດໃສ່ຖານຂໍ້ມູນ. ລວມທັງການປ່ຽນແປງໃນ Entity ທຸລະກໍາທີ່ຕິດຕາມອື່ນແມ່ນຢູ່ໃນທຸລະກໍາດຽວກັນ</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
