namespace DormFlow.Infrastructure.Options;

/// <summary>
/// ຂໍ້ມູນຜູ້ເບິ່ງແຍງລະບົບເລີ່ມຕົ້ນໃຊ້ເມື່ອ Seed
/// </summary>
/// <remarks>
/// ອ່ານຈາກ environment variable ໂດຍກົງໂດຍຊື່ທີ່ກໍານົດໄວ້ໃນເອກະສານລະບົບ
/// ຖ້າບໍ່ໄດ້ລະບຸຢ່າງຄົບຖ້ວນ, ການສ້າງບັນຊີຜູ້ເບິ່ງແຍງລະບົບຈະຖືກຂ້າມໄປ ແລະການແຈ້ງເຕືອນຈະຖືກບັນທຶກ
/// ຢ່າສ້າງບັນຊີດ້ວຍລະຫັດຜ່ານທີ່ຝັງຢູ່ໃນລະຫັດ
/// </remarks>
public sealed class SeedOptions
{
    /// <summary>ຊື່ environment variable ອີເມວຂອງຜູ້ເບິ່ງແຍງລະບົບ</summary>
    public const string AdminEmailVariable = "SEED_ADMIN_EMAIL";

    /// <summary>ຊື່ environment variable ລະຫັດຜ່ານຂອງຜູ້ເບິ່ງແຍງລະບົບ</summary>
    public const string AdminPasswordVariable = "SEED_ADMIN_PASSWORD";

    /// <summary>ຊື່ environment variable ຂອງຊື່ຜູ້ບໍລິຫານ</summary>
    public const string AdminFirstNameVariable = "SEED_ADMIN_FIRST_NAME";

    /// <summary>ຊື່ environment variable ນາມສະກຸນຂອງຜູ້ບໍລິຫານ</summary>
    public const string AdminLastNameVariable = "SEED_ADMIN_LAST_NAME";

    /// <summary>ອີເມວຜູ້ເບິ່ງແຍງລະບົບເລີ່ມຕົ້ນ</summary>
    public string? AdminEmail { get; set; }

    /// <summary>ລະຫັດຜ່ານຂອງຜູ້ເບິ່ງແຍງລະບົບເລີ່ມຕົ້ນ</summary>
    public string? AdminPassword { get; set; }

    /// <summary>ຊື່ຂອງຜູ້ເບິ່ງແຍງລະບົບເລີ່ມຕົ້ນ</summary>
    public string? AdminFirstName { get; set; }

    /// <summary>ນາມສະກຸນຜູ້ເບິ່ງແຍງເລີ່ມຕົ້ນ</summary>
    public string? AdminLastName { get; set; }

    /// <summary>ທ່ານມີຂໍ້ມູນພຽງພໍເພື່ອສ້າງບັນຊີຜູ້ບໍລິຫານບໍ?</summary>
    public bool HasAdminCredentials =>
        !string.IsNullOrWhiteSpace(AdminEmail) && !string.IsNullOrWhiteSpace(AdminPassword);
}
