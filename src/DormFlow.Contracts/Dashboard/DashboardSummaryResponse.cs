using DormFlow.Contracts.Maintenance;

namespace DormFlow.Contracts.Dashboard;

/// <summary>
/// ພາບລວມຂອງ dashboard ລວມຂໍ້ມູນຈາກຫຼາຍໂມດູນເຂົ້າກັນ
/// </summary>
/// <remarks>
/// ທຸກໆພາກສະຜາມແມ່ນ Nullable ຕັ້ງໃຈ: ຜ້າຈໍ shore Frontend ເລືອກເພື່ອສະແດງສະເພາະຊ່ອງຂໍ້ມູນທີ່ກ່ຽວຂ້ອງກັບບົດບາດຂອງຜູ້ໃຊ້ແຕ່ລະຄົນ
/// (ຕົວຢ່າງ, ຊ່າງສ້ອມແປງພຽງແຕ່ໃຊ້ MyAssignedMaintenance) Endpoint ດັ່ງນັ້ນຈຶ່ງຄິດໄລ່ທຸກຊ່ອງຂໍ້ມູນທີ່ເປັນໄປໄດ້ໃນຄໍາຮ້ອງຂໍດຽວ
/// ແທນທີ່ຈະແຍກ Endpoint ສືບຕໍ່ບົດບາດ
/// </remarks>
public class DashboardSummaryResponse
{
    /// <summary>ຈໍານວນຂອງຫ້ອງທີ່ແຍກໂດຍສະຖານະການທີ່ສໍາຄັນແມ່ນຊື່ສະຖານະ (Available/Reserved/Occupied/Cleaning/Maintenance/Inactive)</summary>
    public IReadOnlyDictionary<string, int>? RoomStatusCounts { get; init; }

    /// <summary>ຈຳນວນຫ້ອງທັງໝົດທີ່ຜູ້ໃຊ້ເຂົ້າເຖິງໄດ້</summary>
    public int? TotalRooms { get; init; }

    /// <summary>ອັດຕາການຄອບຄອງ ຄິດໄລ່ເປັນເປີເຊັນ (0-100)</summary>
    public double? OccupancyRate { get; init; }

    /// <summary>ໃບແຈ້ງໜີ້ທີ່ຍັງຄ້າງທັງໝົດ</summary>
    public decimal? OutstandingInvoicesTotal { get; init; }

    /// <summary>ຈຳນວນໃບແຈ້ງໜີ້ທີ່ຍັງຄ້າງຢູ່</summary>
    public int? OutstandingInvoicesCount { get; init; }

    /// <summary>ລາຍຮັບທັງໝົດທີ່ເກັບໄດ້ໃນເດືອນປັດຈຸບັນ</summary>
    public decimal? RevenueThisMonth { get; init; }

    /// <summary>ແນວໂນ້ມລາຍໄດ້ 6 ເດືອນຫຼ້າສຸດ, ຈັດຮຽງຈາກເກົ່າສຸດຫາໃໝ່ສຸດ</summary>
    public IReadOnlyList<RevenueTrendPointResponse>? RevenueTrend { get; init; }

    /// <summary>ຈໍານວນການເຮັດທຸລະກໍາການຈ່າຍເງິນທີ່ລໍຖ້າການຢືນຢັນ</summary>
    public int? PendingPaymentConfirmations { get; init; }

    /// <summary>ຈໍານວນວຽກງານບໍາລຸງຮັກສາທີ່ຍັງບໍ່ທັນສໍາເລັດ (ບໍ່ລວມ Completed/Cancelled)</summary>
    public int? OpenMaintenanceCount { get; init; }

    /// <summary>ວຽກງານບໍາລຸງຮັກສາມອບຝາຍໃຫ້ຜູ້ໃຊ້ປະຈຸບັນ (ບົດບາດຊ່າງສ້ອມແປງເທົ່ານັ້ນ)</summary>
    public IReadOnlyList<MaintenanceRequestResponse>? MyAssignedMaintenance { get; init; }

    /// <summary>ຈໍານວນການຈອງທີ່ຍັງບໍ່ທັນໄດ້ປ່ຽນເປັນສັນຍາແລະໃກ້ຈະຝົດອາຍຸ</summary>
    public int? ExpiringReservationsCount { get; init; }

    /// <summary>ຈໍານວນ parcels ລໍຖ້າໃຫ້ຜູ້ເຊົ່າມາຮັບ</summary>
    public int? ParcelsAwaitingPickup { get; init; }

    /// <summary>ຈໍານວນນັກທ່ອງທ່ຽວທີ່ຍັງຄົງຢູ່ໃນຫໍພັກ (ເຊັກອິນແຕ່ຍັງບໍ່ທັນໄດ້ເຊັກເອົາ)</summary>
    public int? ActiveVisitorsCount { get; init; }

    /// <summary>ການເຄື່ອນໄຫວຫຼ້າສຸດໃນລະບົບ (ຈາກ Audit Log) ຈັດລຽງຈາກໃໝ່ສຸດຫາເກົ່າທີ່ສຸດ</summary>
    public IReadOnlyList<DashboardActivityItemResponse>? RecentActivity { get; init; }
}

/// <summary>ຈຸດຂໍ້ມູນອັນໜຶ່ງຂອງກາຟແນວໂນ້ມລາຍໄດ້</summary>
public class RevenueTrendPointResponse
{
    /// <summary>ປ້າຍກຳກັບຈຸດຂໍ້ມູນ (ເດືອນ/ປີ)</summary>
    public string Label { get; init; } = string.Empty;

    /// <summary>ລາຍຮັບທັງໝົດຂອງເດືອນນັ້ນ</summary>
    public decimal Amount { get; init; }
}

/// <summary>ຜຶ່ງໃນກິດຈະກໍາທີ່ຈະສະແດງຢູ່ໃນ dashboard ໄດ້</summary>
public class DashboardActivityItemResponse
{
    /// <summary>ລະຫັດອ້າງອີງເຫດການ (ມາຈາກ Audit Log)</summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>ຂໍ້ຄວາມສະຫຼຸບຂອງເຫດການ</summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>ເວລາຂອງເຫດການ</summary>
    public DateTimeOffset OccurredAt { get; init; }
}
