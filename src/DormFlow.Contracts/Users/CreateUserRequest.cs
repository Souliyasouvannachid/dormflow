namespace DormFlow.Contracts.Users;

/// <summary>ຮ້ອງຂໍເພື່ອສ້າງບັນຊີຜູ້ໃຊ້ໃຝ່</summary>
public class CreateUserRequest
{
    /// <summary>
    /// ອົງກອນທີ່ຜູ້ໃຊ້ເປັນຂອງ Effective ພຽງແຕ່ຖ້າຜູ້ໂທແມ່ນ SuperAdmin ເທົ່ານັ້ນ
    /// ຜູ້ໃຊ້ອື່ນຖືກບັງຄັບໃຫ້ສ້າງບັນຊີຢູ່ໃນອົງກອນຂອງພວກເຂົາສະເຝີວ່າຄ່ານີ້ຖືກກໍານົດຫຼືບໍ່
    /// </summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>ອີເມວທີ່ໃຊ້ເພື່ອເຂົ້າສູ່ລະບົບເປັນເອກະລັກທົ່ວລະບົບ</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>ລະຫັດຜ່ານເລີ່ມຕົ້ນ</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>ຊື່</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>ນາມສະກຸນ</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>ເບີໂທລະສັບ</summary>
    public string? Phone { get; set; }
}
