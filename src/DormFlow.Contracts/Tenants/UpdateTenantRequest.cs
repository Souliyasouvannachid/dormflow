namespace DormFlow.Contracts.Tenants;

/// <summary>ຮ້ອງຂໍແກ້ໄຂຂໍ້ມູນຜູ້ເຊົ່າ</summary>
public class UpdateTenantRequest
{
    /// <summary>ຊື່</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>ນາມສະກຸນ</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>ອີເມວ</summary>
    public string? Email { get; set; }

    /// <summary>ເບີໂທລະສັບ</summary>
    public string? Phone { get; set; }

    /// <summary>ບັດປະຈຳຕົວ ຫຼືໝາຍເລກໜັງສືຜ່ານແດນ</summary>
    public string? NationalId { get; set; }

    /// <summary>ວັນເດືອນປີເກີດ</summary>
    public DateOnly? DateOfBirth { get; set; }

    /// <summary>ທີ່ຢູ່ ຕາມທະບຽນເຮືອນ</summary>
    public string? Address { get; set; }

    /// <summary>ຊື່ຂອງຜູ້ຕິດຕໍ່ສຸກເສີນ</summary>
    public string? EmergencyContactName { get; set; }

    /// <summary>ເບີໂທລະສັບຕິດຕໍ່ສຸກເສີນ</summary>
    public string? EmergencyContactPhone { get; set; }
}
