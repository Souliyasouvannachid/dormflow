using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.AuditLogs;

/// <summary>ຕົວກໍານົດການຊອກຫາລາຍການ Audit Log</summary>
public class AuditLogListRequest : PagedQueryRequest
{
    /// <summary>
    /// ການກັ່ນຕອງພຽງແຕ່ເຫດການຂອງອົງກອນທີ່ລະບຸ. ມີປະສິດພາບພຽງແຕ່ຖ້າຜູ້ໂທແມ່ນ SuperAdmin
    /// ເຫດການໃນອົງກອນຂອງຕົນເອງແມ່ນເຫັນໄດ້ໂດຍຜູ້ໃຊ້ອື່ນສະເໝີວ່າຄ່ານີ້ຖືກລະບຸຫຼືບໍ່
    /// </summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະເຫດການສຳລັບຫໍພັກທີ່ລະບຸໄວ້ເທົ່ານັ້ນ</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະເຫດການທີ່ລິເລີ່ມໂດຍຜູ້ໃຊ້ນີ້</summary>
    public Guid? UserId { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະເຫດການຂອງປະເພດທີ່ລະບຸ</summary>
    public string? Action { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະເຫດການທີ່ກ່ຽວຂ້ອງກັບ Entity ຊື່ນີ້</summary>
    public string? EntityName { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະເຫດການທີ່ເກີດຂຶ້ນຕັ້ງແຕ່ເວລານີ້ເປັນຕົ້ນໄປ</summary>
    public DateTimeOffset? From { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະເຫດການທີ່ເກີດຂຶ້ນກ່ອນ ຫຼືເທົ່າກັບເວລານີ້</summary>
    public DateTimeOffset? To { get; set; }
}
