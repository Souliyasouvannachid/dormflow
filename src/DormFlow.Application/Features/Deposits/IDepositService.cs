using DormFlow.Contracts.Common;
using DormFlow.Contracts.Deposits;

namespace DormFlow.Application.Features.Deposits;

/// <summary>Use Case ໂມດູນເງິນຝາກທັງໝົດ</summary>
public interface IDepositService
{
    /// <summary>ດຶງເອົາລາຍຊື່ເງິນຝາກທີ່ແບ່ງອອກເປັນໜ້າ</summary>
    Task<PagedResult<DepositResponse>> GetPagedAsync(
        DepositListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນເງິນຝາກຈາກລະຫັດການສົ່ງຕໍ່</summary>
    Task<DepositResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ບັນທຶກການໄດ້ຮັບເງິນຝາກໃຝ່ ສັນຍາເຊົ່າຜຶ່ງສາມາດໄດ້ຮັບການຍອມຮັບພຽງແຕ່ຄັ້ງດຽວ</summary>
    Task<DepositResponse> CollectAsync(
        CollectDepositRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ການຄືນເງິນເງິນຝາກສາມາດໃຊ້ໄດ້ພຽງແຕ່ສໍາລັບການເຊົ່າທີ່ສິ້ນສຸດລົງແລະເງິນຝາກທີ່ຍັງບໍ່ໄດ້ສົ່ງຄືນ. ນີ້ສາມາດເຮັດໄດ້ພຽງແຕ່ຜຶ່ງຄັ້ງຕໍ່ລາຍການ
    /// </summary>
    Task<DepositResponse> RefundAsync(
        Guid id,
        RefundDepositRequest request,
        CancellationToken cancellationToken = default);
}
