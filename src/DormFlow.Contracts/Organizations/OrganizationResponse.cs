namespace DormFlow.Contracts.Organizations;

/// <summary>ຂໍ້ມູນຂອງອົງກອນຜຶ່ງ</summary>
public class OrganizationResponse
{
    /// <summary>ລະຫັດອ້າງອີງອົງກອນ</summary>
    public Guid Id { get; init; }

    /// <summary>ລະຫັດອົງກອນ</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>ຊື່ອົງກອນ</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>ໝາຍເລກປະຈຳຕົວຜູ້ເສຍພາສີ</summary>
    public string? TaxId { get; init; }

    /// <summary>ເບີໂທລະສັບຕິດຕໍ່</summary>
    public string? Phone { get; init; }

    /// <summary>ອີເມວຕິດຕໍ່</summary>
    public string? Email { get; init; }

    /// <summary>ທີ່ຢູ່</summary>
    public string? Address { get; init; }

    /// <summary>ສະຖານະອົງກອນ</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>ເວລາສ້າງຂໍ້ມູນ</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>ເວລາຂອງການດັດແກ້ຫຼ້າສຸດ</summary>
    public DateTimeOffset? UpdatedAt { get; init; }
}
