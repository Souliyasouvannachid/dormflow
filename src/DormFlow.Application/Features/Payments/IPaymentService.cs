using DormFlow.Contracts.Common;
using DormFlow.Contracts.Payments;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Payments;

/// <summary>Use Case ໂມດູນການຈ່າຍເງິນທັງຝົດ</summary>
public interface IPaymentService
{
    /// <summary>ດຶງເອົາລາຍການການຈ່າຍເງິນທີ່ໄດ້ວາງໄວ້</summary>
    Task<PagedResult<PaymentResponse>> GetPagedAsync(
        PaymentListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ນັບລາຍການຊຳລະຕາມສະຖານະ</summary>
    Task<int> GetCountAsync(PaymentStatus status, CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນການຈ່າຍເງິນຈາກລະຫັດການສົ່ງຕໍ່</summary>
    Task<PaymentResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// ບັນທຶກການຈ່າຍເງິນໃໝ່ໃນສະຖານະການຢືນຢັນທີ່ຍັງຄ້າງຢູ່. ຈຳນວນໃບແຈ້ງໜີ້ຈະບໍ່ໄດ້ຮັບຜົນກະທົບຈົນກວ່າຈະຢືນຢັນ
    /// </summary>
    Task<PaymentResponse> CreateAsync(
        CreatePaymentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ຢືນຢັນລາຍການການຈ່າຍເງິນ ລົບຈໍານວນອອກຈາກໃບແຈ້ງໜີ້ ແລະປັບສະຖານະໃບແຈ້ງໜີ້ຕາມຍອດເງິນທີ່ຍັງເຫຼືອ</summary>
    Task<PaymentResponse> ConfirmAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ການຄືນເງິນແມ່ນມີໃຫ້ສຳລັບລາຍການທີ່ຢືນຢັນແລ້ວເທົ່ານັ້ນ. ຈຳນວນເງິນຈະຖືກຫັກອອກຈາກໃບແຈ້ງໜີ້ການຄືນເງິນ</summary>
    Task<PaymentResponse> RefundAsync(
        Guid id,
        RefundPaymentRequest request,
        CancellationToken cancellationToken = default);
}
