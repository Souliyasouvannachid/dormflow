namespace DormFlow.Contracts.Roles;

/// <summary>ຮ້ອງຂໍການແກ້ໄຂຂໍ້ມູນບົດບາດ</summary>
/// <remarks>ລະບົບປະຕິເສດການປ່ຽນຊື່ຂອງບົດບາດມາດຕະຖານ. ເຖິງແມ່ນວ່າຄ່າອື່ນຖືກລະບຸໄວ້ໃນພາກສະຜາມ Name ແນວໃດກໍ່ຕາມ</remarks>
public class UpdateRoleRequest
{
    /// <summary>ຊື່ບົດບາດແມ່ນເປັນເອກະລັກໃນທົ່ວລະບົບ</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>ລາຍລະອຽດບົດບາດ</summary>
    public string? Description { get; set; }
}
