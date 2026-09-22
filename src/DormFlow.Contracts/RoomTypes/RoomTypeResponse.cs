namespace DormFlow.Contracts.RoomTypes;

/// <summary>ປະເພດຂອງຂໍ້ມູນຫ້ອງ</summary>
public class RoomTypeResponse
{
    /// <summary>ລະຫັດອ້າງອີງປະເພດຫ້ອງ</summary>
    public Guid Id { get; init; }

    /// <summary>ອົງກອນເຈົ້າຂອງຫ້ອງ</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>ຫໍພັກທີ່ປະເພດຫ້ອງນີ້ຂຶ້ນກັບ</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ລະຫັດປະເພດຫ້ອງ</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>ຊື່ປະເພດຫ້ອງ</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>ຄໍາອະທິບາຍເພີ່ມເຕີມ</summary>
    public string? Description { get; init; }

    /// <summary>ຄ່າເຊົ່າລາຍເດືອນ</summary>
    public decimal MonthlyRent { get; init; }

    /// <summary>ຄ່າເຊົ່າປະຈໍາວັນ</summary>
    public decimal? DailyRent { get; init; }

    /// <summary>ຝາກ</summary>
    public decimal DepositAmount { get; init; }

    /// <summary>ຈຳນວນແຂກສູງສຸດ</summary>
    public int MaximumOccupants { get; init; }

    /// <summary>ສະຖານະປະເພດຫ້ອງ</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>ເວລາສ້າງຂໍ້ມູນ</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>ເວລາຂອງການດັດແກ້ຫຼ້າສຸດ</summary>
    public DateTimeOffset? UpdatedAt { get; init; }
}
