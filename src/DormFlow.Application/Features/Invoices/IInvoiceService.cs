using DormFlow.Contracts.Common;
using DormFlow.Contracts.Invoices;

namespace DormFlow.Application.Features.Invoices;

/// <summary>Use Case ໂມດູນໃບແຈ້ງໜີ້ທັງໝົດ</summary>
public interface IInvoiceService
{
    /// <summary>ດຶງເອົາລາຍຊື່ໃບແຈ້ງໜີ້ທີ່ໄດ້ວາງໄວ້ແລ້ວ</summary>
    Task<PagedResult<InvoiceResponse>> GetPagedAsync(
        InvoiceListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນໃບແຈ້ງໜີ້ຈາກລະຫັດອ້າງອີງ</summary>
    Task<InvoiceResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// ສ້າງໃບແຈ້ງໜີ້ໃໝ່ໃນສະຖານະສະບັບຮ່າງ. ຄິດໄລ່ໃບບິນຄ່ານ້ໍາ-ໄຟຟ້າຈາກຄ່າແມັດຂອງເດືອນທີ່ກໍານົດອັດຕະໂນມັດ
    /// </summary>
    Task<InvoiceResponse> CreateAsync(
        CreateInvoiceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ອອກໃບແຈ້ງໜີ້ໃຫ້ຜູ້ເຊົ່າ (Draft → Issued)</summary>
    Task<InvoiceResponse> IssueAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ຍົກເລີກໃບແຈ້ງໜີ້ ນີ້ສາມາດເຮັດໄດ້ພຽງແຕ່ຖ້າຍັງບໍ່ໄດ້ຮັບການຈ່າຍເງິນເທື່ອ</summary>
    Task<InvoiceResponse> CancelAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// ໝາຍໃບແຈ້ງໜີ້ທີ່ຜ່ານວັນທີຮອດກຳນົດ (Issued/PartiallyPaid ທີ່ DueDate ມັນໄດ້ຜ່ານໄປ) ໃຫ້ມັນເປັນ
    /// Overdue ຄຽງຄູ່ກັບການຄິດໄລ່ຄ່າປັບໃໝການຊໍາລະຊ້າຕາມການກຳນົດຂອງຫໍພັກຄັ້ງດຽວ ເມື່ອປ່ຽນສະຖານະ, ເອີ້ນໂດຍ Background Job
    /// ແຕ່ລະໄລຍະ, ເຮັດວຽກທົ່ວທຸກອົງກອນ. ບໍ່ມີການກວດສອບຜູ້ໃຊ້
    /// </summary>
    /// <returns>ຈຳນວນລາຍການທີ່ຖືກໝາຍວ່າໝົດກຳນົດໃນການໂທນີ້</returns>
    Task<int> MarkOverdueInvoicesAsync(CancellationToken cancellationToken = default);
}
