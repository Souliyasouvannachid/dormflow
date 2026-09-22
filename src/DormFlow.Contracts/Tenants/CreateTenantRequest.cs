namespace DormFlow.Contracts.Tenants;

/// <summary>ຮ້ອງຂໍການສ້າງຂໍ້ມູນຜູ້ເຊົ່າໃຝ່</summary>
public class CreateTenantRequest
{
    /// <summary>
    /// ອົງກອນທີ່ຜູ້ເຊົ່າຈະເປັນຂອງ Effective ພຽງແຕ່ຖ້າຜູ້ໂທແມ່ນ SuperAdmin
    /// ຜູ້ໃຊ້ອື່ນຖືກບັງຄັບໃຫ້ສ້າງຢູ່ໃນອົງກອນຂອງພວກເຂົາສະເຝີວ່າຄ່ານີ້ຖືກກໍານົດຫຼືບໍ່
    /// </summary>
    public Guid? OrganizationId { get; set; }

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
