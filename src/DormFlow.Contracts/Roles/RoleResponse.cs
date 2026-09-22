namespace DormFlow.Contracts.Roles;

/// <summary>ຂໍ້ມູນບົດບາດຜຶ່ງ</summary>
public class RoleResponse
{
    /// <summary>ລະຫັດການສົ່ງຕໍ່ບົດບາດ</summary>
    public Guid Id { get; init; }

    /// <summary>ຊື່ບົດບາດ</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>ລາຍລະອຽດບົດບາດ</summary>
    public string? Description { get; init; }

    /// <summary>
    /// ນີ້ແມ່ນບົດບາດມາດຕະຖານສໍາລັບລະບົບບໍ?
    /// ບົດບາດມາດຕະຖານບໍ່ສາມາດຖືກປ່ຽນຊື່ ແລະລຶບອອກໄດ້. ແຕ່ທ່ານສາມາດປັບສິດຂອງທ່ານໄດ້ຕາມປົກກະຕິ
    /// </summary>
    public bool IsSystemRole { get; init; }

    /// <summary>ລະຫັດການອະນຸຍາດທັງໝົດທີ່ບົດບາດນີ້ມີ</summary>
    public IReadOnlyList<string> Permissions { get; init; } = [];
}
