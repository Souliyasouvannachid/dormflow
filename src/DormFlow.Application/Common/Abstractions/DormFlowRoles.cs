namespace DormFlow.Application.Common.Abstractions;

/// <summary>
/// ບົດບາດຜູ້ໃຊ້ທັງຝົດຂອງລະບົບ
/// </summary>
public static class DormFlowRoles
{
    /// <summary>ຜູ້ບໍລິຫານລະດັບສູງ ເຂົ້າເຖິງຂໍ້ມູນຈາກທຸກອົງກອນ</summary>
    public const string SuperAdmin = "SuperAdmin";

    /// <summary>ເຈົ້າຂອງທຸລະກິດສາມາດເບິ່ງແຍງທຸກໆຫໍພັກໃນອົງກອນຂອງພວກເຂົາ</summary>
    public const string Owner = "Owner";

    /// <summary>ຜູ້ຈັດການຫໍພັກດູແລພຽງແຕ່ຫໍພັກທີ່ໄດ້ຮັບມອບຝາຍ</summary>
    public const string DormManager = "DormManager";

    /// <summary>ພະແນກບັນຊີ</summary>
    public const string Accountant = "Accountant";

    /// <summary>ນາຍພົນ</summary>
    public const string Staff = "Staff";

    /// <summary>ຊ່າງສ້ອມແປງ</summary>
    public const string Technician = "Technician";

    /// <summary>ຜູ້ເຊົ່າເຫັນພຽງແຕ່ຂໍ້ມູນຂອງຕົນເອງ</summary>
    public const string Tenant = "Tenant";

    /// <summary>ບັນຊີລາຍຊື່ຂອງບົດບາດທັງຝົດ, ໃຊ້ຕອນ Seed ຂໍ້ມູນເລີ່ມຕົ້ນ</summary>
    public static IReadOnlyList<string> All { get; } =
    [
        SuperAdmin,
        Owner,
        DormManager,
        Accountant,
        Staff,
        Technician,
        Tenant,
    ];
}
