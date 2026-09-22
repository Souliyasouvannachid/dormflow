namespace DormFlow.Domain.Common;

/// <summary>
/// Entity ຜູກມັດກັບອົງກອນ (Tenant) ທຸກໆ Query ຕ້ອງໄດ້ກັ່ນຕອງຄືກັນ <see cref="OrganizationId"/>
/// ແລະການກັ່ນຕອງເຊັ່ນດຽວກັນ <see cref="DormitoryId"/> ເພີ່ມເມື່ອຂໍ້ມູນຖືກຜູກມັດກັບຫໍພັກສະເພາະ
/// </summary>
public interface ITenantScoped
{
    /// <summary>ອົງກອນເຈົ້າຂອງຂໍ້ມູນ</summary>
    Guid OrganizationId { get; set; }

    /// <summary>ຫໍພັກທີ່ເປັນເຈົ້າຂອງຂໍ້ມູນແມ່ນ null ເມື່ອຂໍ້ມູນຢູ່ໃນລະດັບວິສາຫະກິດ</summary>
    Guid? DormitoryId { get; set; }
}
