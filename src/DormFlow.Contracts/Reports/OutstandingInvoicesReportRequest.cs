using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Reports;

/// <summary>ພາລາມິເຕີຂອງບົດລາຍງານໃບແຈ້ງໜີ້ທີ່ບໍ່ໄດ້ຮັບຄ່າຈ້າງ</summary>
public class OutstandingInvoicesReportRequest : PagedQueryRequest
{
    /// <summary>ກັ່ນຕອງສະເພາະຫໍພັກທີ່ລະບຸເປັນ null ເມື່ອເຈົ້າຕ້ອງການທຸກຫໍພັກທີ່ຜູ້ໂທມີການເຂົ້າເຖິງ</summary>
    public Guid? DormitoryId { get; set; }
}
