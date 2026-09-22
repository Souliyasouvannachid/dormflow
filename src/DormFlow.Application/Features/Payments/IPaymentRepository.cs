using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Payments;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງຂໍ້ມູນການຈ່າຍເງິນແບບ paginated</summary>
public sealed record PaymentListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? InvoiceId,
    PaymentStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນການຈ່າຍເງິນໃນຖານຂໍ້ມູນ</summary>
public interface IPaymentRepository
{
    /// <summary>ອ່ານລາຍການການຈ່າຍເງິນຈາກລະຫັດການສົ່ງຕໍ່</summary>
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາບັນຊີລາຍຈ່າຍ paginated ໂດຍອີງໃສ່ຕົວກອງທີ່ລະບຸ</summary>
    Task<PagedResult<Payment>> GetPagedAsync(
        PaymentListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>ນັບລາຍການຊຳລະຕາມຕົວກອງໂດຍບໍ່ດຶງລາຍການ</summary>
    Task<int> CountAsync(PaymentListFilter filter, CancellationToken cancellationToken = default);

    /// <summary>ເພີ່ມລາຍການການຈ່າຍເງິນໃໝ່</summary>
    Task AddAsync(Payment payment, CancellationToken cancellationToken = default);

    /// <summary>ບັນທຶກການປ່ຽນແປງທັງໝົດໃສ່ຖານຂໍ້ມູນ. ລວມທັງຈໍານວນໃບເກັບເງິນທີ່ຖືກແກ້ໄຂໃນທຸລະກໍາດຽວກັນ</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
