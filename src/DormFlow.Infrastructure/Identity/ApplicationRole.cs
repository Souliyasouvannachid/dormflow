using Microsoft.AspNetCore.Identity;

namespace DormFlow.Infrastructure.Identity;

/// <summary>ບົດບາດຂອງຜູ້ໃຊ້ສືບຕໍ່ຈາກ <see cref="IdentityRole{TKey}"/></summary>
public class ApplicationRole : IdentityRole<Guid>
{
    /// <summary>ສ້າງບົດບາດຫວ່າງເປົ່າ. ໃຊ້ໂດຍ EF Core ເມື່ອສ້າງວັດຖຸຈາກຖານຂໍ້ມູນ</summary>
    public ApplicationRole()
    {
    }

    /// <summary>ສ້າງບົດບາດຈາກຊື່</summary>
    /// <param name="roleName">ຊື່ບົດບາດ</param>
    public ApplicationRole(string roleName)
        : base(roleName)
    {
    }

    /// <summary>ລາຍລະອຽດບົດບາດ</summary>
    public string? Description { get; set; }

    /// <summary>
    /// ນີ້ແມ່ນບົດບາດມາດຕະຖານສໍາລັບລະບົບບໍ?
    /// ບໍ່ສາມາດລຶບບົດບາດທີ່ເປັນຂອງລະບົບໄດ້. ເນື່ອງຈາກວ່າມັນຖືກອ້າງອີງໃນລະຫັດແລະຂໍ້ມູນເບື້ອງຕົ້ນ
    /// </summary>
    public bool IsSystemRole { get; set; }
}
