namespace DormFlow.Contracts.Settings;

/// <summary>ຂໍ້ມູນການຕັ້ງຄ່າຜຶ່ງ</summary>
public class SettingResponse
{
    /// <summary>ລະຫັດອ້າງອີງການຕັ້ງຄ່າ</summary>
    public Guid Id { get; init; }

    /// <summary>ອົງກອນທີ່ເປັນເຈົ້າຂອງການຕັ້ງຄ່ານີ້</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>ການຕັ້ງຄ່າຊື່ຄີ</summary>
    public string Key { get; init; } = string.Empty;

    /// <summary>ການຕັ້ງຄ່າ</summary>
    public string Value { get; init; } = string.Empty;

    /// <summary>ເວລາສ້າງຂໍ້ມູນ</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>ເວລາດັດແກ້ຫຼ້າສຸດ</summary>
    public DateTimeOffset? UpdatedAt { get; init; }
}
