namespace DormFlow.Contracts.Buildings;

/// <summary>ຂໍ້ມູນການກໍ່ສ້າງຜຶ່ງ</summary>
public class BuildingResponse
{
    /// <summary>ລະຫັດອ້າງອີງອາຄານ</summary>
    public Guid Id { get; init; }

    /// <summary>ອົງກອນເຈົ້າຂອງການກໍ່ສ້າງ</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>ຫໍພັກທີ່ຕຶກນີ້ເປັນຂອງ</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ລະຫັດອາຄານ</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>ຊື່ອາຄານ</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>ຄໍາອະທິບາຍເພີ່ມເຕີມ</summary>
    public string? Description { get; init; }

    /// <summary>ຈໍານວນຊັ້ນທັງຝົດຂອງອາຄານ</summary>
    public int TotalFloors { get; init; }

    /// <summary>ສະຖານະຂອງອາຄານ</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>ເວລາສ້າງຂໍ້ມູນ</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>ເວລາຂອງການດັດແກ້ຫຼ້າສຸດ</summary>
    public DateTimeOffset? UpdatedAt { get; init; }
}
