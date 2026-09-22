using DormFlow.Domain.Common;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ການຕັ້ງຄ່າອົງກອນຜຶ່ງ, ເກັບຮັກສາໄວ້ເປັນຄູ່ Key/Value ເອກະລາດ
/// </summary>
/// <remarks>
/// ສືບທອດມາຈາກ <see cref="BaseEntity"/> ບໍ່ <see cref="TenantEntity"/> ເນື່ອງຈາກວ່າການຕັ້ງຄ່າແມ່ນພຽງແຕ່ຢູ່ໃນລະດັບອົງກອນ
/// ບໍ່ໄດ້ຜູກມັດກັບຫໍພັກໃດນຶ່ງ <see cref="Tenant"/> — Key ບໍ່ເປັນເອກະລັກພາຍໃນອົງກອນດຽວກັນ
/// (unique index ສຸດ OrganizationId+Key) ຮັກສາມັນເປັນ Key/Value ເປັນເອກະລາດແທນທີ່ຈະມີຖັນຄົງທີ່ຕັ້ງເປັນສ່ວນບຸກຄົນ
/// ເນື່ອງຈາກວ່າເອກະສານຕົ້ນສະບັບບໍ່ໄດ້ລະບຸບັນຊີລາຍຊື່ຂອງການຕັ້ງຄ່າທີ່ກໍານົດໄວ້ທັງຝົດ
/// </remarks>
public class Setting : BaseEntity
{
    /// <summary>ອົງກອນທີ່ເປັນເຈົ້າຂອງການຕັ້ງຄ່ານີ້</summary>
    public Guid OrganizationId { get; set; }

    /// <summary>ການຕັ້ງຄ່າຊື່ຄີ</summary>
    public required string Key { get; set; }

    /// <summary>ການຕັ້ງຄ່າການເກັບຮັກສາເປັນຂໍ້ຄວາມຟຣີ (ອາດຈະເປັນ JSON ສິດທັງໝົດ)</summary>
    public required string Value { get; set; }
}
