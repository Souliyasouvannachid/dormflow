namespace DormFlow.Domain.Entities;

/// <summary>
/// ຄວາມສຳພັນລະຫວ່າງບົດບາດ ແລະ ສິດ
/// </summary>
/// <remarks>
/// ໃຊ້ Composite Key ຂອງ RoleId ແລະ PermissionId ດັ່ງນັ້ນບໍ່ຈໍາເປັນຕ້ອງມີຖັນ Id
/// ແລະບໍ່ໃຊ້ Soft Delete ເນື່ອງຈາກວ່າການຖອນການອະນຸຍາດຕົວຈິງແລ້ວຄວນລຶບແຖວ
/// </remarks>
public class RolePermission
{
    /// <summary>ບົດບາດສິດພິເສດ</summary>
    public Guid RoleId { get; set; }

    /// <summary>ອະນຸຍາດ</summary>
    public Guid PermissionId { get; set; }

    /// <summary>ເວລາອະນຸຍາດ</summary>
    public DateTimeOffset GrantedAt { get; set; }

    /// <summary>ຜູ້ໃຫ້ທຶນແມ່ນ null ເມື່ອໃຫ້ໂດຍລະບົບບົດ Seed</summary>
    public Guid? GrantedBy { get; set; }

    /// <summary>ສິດອ້າງເຖິງ</summary>
    public Permission? Permission { get; set; }
}
