namespace DormFlow.Contracts.Organizations;

/// <summary>ຮ້ອງຂໍການແກ້ໄຂຂໍ້ມູນອົງກອນ</summary>
public class UpdateOrganizationRequest
{
    /// <summary>ລະຫັດອົງກອນແມ່ນເປັນເອກະລັກໃນທົ່ວລະບົບ</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>ຊື່ອົງກອນ</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>ໝາຍເລກປະຈຳຕົວຜູ້ເສຍພາສີ</summary>
    public string? TaxId { get; set; }

    /// <summary>ເບີໂທລະສັບຕິດຕໍ່</summary>
    public string? Phone { get; set; }

    /// <summary>ອີເມວຕິດຕໍ່</summary>
    public string? Email { get; set; }

    /// <summary>ທີ່ຢູ່</summary>
    public string? Address { get; set; }
}
