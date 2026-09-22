namespace DormFlow.Contracts.Floors;

/// <summary>ຂໍ້ມູນຊັ້ນທໍາອິດຂອງອາຄານ</summary>
public class FloorResponse
{
    /// <summary>ລະຫັດອ້າງອີງຊັ້ນ</summary>
    public Guid Id { get; init; }

    /// <summary>ອົງກອນທີ່ເປັນເຈົ້າຂອງຫ້ອງຮຽນນີ້</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>ຫໍພັກຊັ້ນນີ້ຂຶ້ນກັບ</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ອາຄານທີ່ຊັ້ນນີ້ຂຶ້ນກັບ</summary>
    public Guid BuildingId { get; init; }

    /// <summary>ລຳດັບ</summary>
    public int FloorNumber { get; init; }

    /// <summary>ຊື່ຫ້ອງຮຽນສໍາລັບການສະແດງ</summary>
    public string? Name { get; init; }

    /// <summary>ຄໍາອະທິບາຍເພີ່ມເຕີມ</summary>
    public string? Description { get; init; }

    /// <summary>ສະຖານະຊັ້ນ</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>ເວລາສ້າງຂໍ້ມູນ</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>ເວລາຂອງການດັດແກ້ຫຼ້າສຸດ</summary>
    public DateTimeOffset? UpdatedAt { get; init; }
}
