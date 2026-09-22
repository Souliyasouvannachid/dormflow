using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;

namespace DormFlow.Application.Features.Receipts;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງບັນຊີລາຍຊື່ paginated ຂອງໃບຮັບ</summary>
public sealed record ReceiptListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? PaymentId,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນໃບຮັບເງິນໃນຖານຂໍ້ມູນ</summary>
public interface IReceiptRepository
{
    /// <summary>ອ່ານໃບຮັບເງິນຈາກລະຫັດອ້າງອີງ</summary>
    Task<Receipt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ກວດເບິ່ງວ່າໃບຮັບເງິນໄດ້ຖືກອອກສໍາລັບການຈ່າຍເງິນນີ້ຫຼືບໍ່</summary>
    Task<bool> ExistsByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default);

    /// <summary>ກວດເບິ່ງວ່າມີໃບຮັບເງິນທີ່ໃຊ້ຕົວເລກນີ້ຢູ່ໃນອົງກອນດຽວກັນຫຼືບໍ່</summary>
    Task<bool> ExistsByReceiptNumberAsync(
        Guid organizationId,
        string receiptNumber,
        CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາລາຍຊື່ໃບຮັບເງິນທີ່ໄດ້ວາງໄວ້ຕາມຕົວກອງທີ່ລະບຸໄວ້</summary>
    Task<PagedResult<Receipt>> GetPagedAsync(
        ReceiptListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>ເພີ່ມໃບຮັບເງິນໃໝ່</summary>
    Task AddAsync(Receipt receipt, CancellationToken cancellationToken = default);

    /// <summary>ບັນທຶກການປ່ຽນແປງທັງໝົດໃສ່ຖານຂໍ້ມູນ</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
