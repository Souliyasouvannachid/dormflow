namespace DormFlow.Contracts.Tenants;

/// <summary>ຂໍ້ມູນຜູ້ເຊົ່າຜຶ່ງ</summary>
public class TenantResponse
{
    /// <summary>ລະຫັດອ້າງອີງຜູ້ເຊົ່າ</summary>
    public Guid Id { get; init; }

    /// <summary>ອົງກອນທີ່ຜູ້ເຊົ່ານີ້ຂຶ້ນກັບ</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>ຊື່</summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>ນາມສະກຸນ</summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>ອີເມວ</summary>
    public string? Email { get; init; }

    /// <summary>ເບີໂທລະສັບ</summary>
    public string? Phone { get; init; }

    /// <summary>ບັດປະຈຳຕົວ ຫຼືໝາຍເລກໜັງສືຜ່ານແດນ</summary>
    public string? NationalId { get; init; }

    /// <summary>ວັນເດືອນປີເກີດ</summary>
    public DateOnly? DateOfBirth { get; init; }

    /// <summary>ທີ່ຢູ່ ຕາມທະບຽນເຮືອນ</summary>
    public string? Address { get; init; }

    /// <summary>ຊື່ຂອງຜູ້ຕິດຕໍ່ສຸກເສີນ</summary>
    public string? EmergencyContactName { get; init; }

    /// <summary>ເບີໂທລະສັບຕິດຕໍ່ສຸກເສີນ</summary>
    public string? EmergencyContactPhone { get; init; }

    /// <summary>ສະຖານະຜູ້ເຊົ່າ</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>ເວລາສ້າງຂໍ້ມູນ</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>ເວລາຂອງການດັດແກ້ຫຼ້າສຸດ</summary>
    public DateTimeOffset? UpdatedAt { get; init; }
}
