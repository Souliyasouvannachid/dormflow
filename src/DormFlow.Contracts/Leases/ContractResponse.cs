namespace DormFlow.Contracts.Leases;

/// <summary>ຂໍ້ມູນການເຊົ່າຜຶ່ງ</summary>
public class ContractResponse
{
    public decimal? WaterRatePerUnit { get; init; }
    public decimal? ElectricityRatePerUnit { get; init; }
    public Guid[] CoTenantIds { get; init; } = [];

    /// <summary>ລະຫັດອ້າງອີງສັນຍາ</summary>
    public Guid Id { get; init; }

    /// <summary>ອົງກອນທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>ຫໍພັກທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ຫ້ອງສັນຍາ</summary>
    public Guid RoomId { get; init; }

    /// <summary>ຜູ້ເຊົ່າຕາມສັນຍາ</summary>
    public Guid TenantId { get; init; }

    /// <summary>ການຈອງທີ່ເຂົ້າມາໃນສັນຍານີ້ແມ່ນ null ໃນເວລາທີ່ເຮັດສັນຍາໂດຍບໍ່ມີການເຮັດການຈອງລ່ວງຜ້າ</summary>
    public Guid? ReservationId { get; init; }

    /// <summary>ໝາຍເລກສັນຍາ</summary>
    public string ContractNumber { get; init; } = string.Empty;

    /// <summary>ວັນທີເລີ່ມຕົ້ນຂອງສັນຍາ</summary>
    public DateOnly StartDate { get; init; }

    /// <summary>ກຳນົດວັນທີສິ້ນສຸດສັນຍາ</summary>
    public DateOnly EndDate { get; init; }

    /// <summary>ເຊົ່າລາຍເດືອນຕາມສັນຍາສະບັບນີ້</summary>
    public decimal MonthlyRent { get; init; }

    /// <summary>ຝາກເງິນຕາມສັນຍານີ້</summary>
    public decimal DepositAmount { get; init; }

    /// <summary>ສະຖານະສັນຍາ</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>ເວລາເປີດໃຊ້ສັນຍາ (ເຊັກອິນ)</summary>
    public DateTimeOffset? SignedAt { get; init; }

    /// <summary>ເວລາສິ້ນສຸດສັນຍາ (ກວດເບິ່ງ)</summary>
    public DateTimeOffset? TerminatedAt { get; init; }

    /// <summary>ເຫດຜົນສໍາລັບການຍົກເລີກສັນຍາ</summary>
    public string? TerminationReason { get; init; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; init; }

    /// <summary>ເວລາສ້າງຂໍ້ມູນ</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
