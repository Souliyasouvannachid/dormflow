namespace DormFlow.Domain.Common;

/// <summary>
/// Entity ທີ່ຕ້ອງບັນທຶກຮ່ອງຮອຍຂອງການສ້າງແລະການດັດແກ້
/// ຄຸນຄ່າເຫຼົ່ານີ້ຖືກບັນຈຸໂດຍອັດຕະໂນມັດໂດຍ Interceptor ຂອງ EF Core ມັນບໍ່ຄວນຈະຖືກປັບແຕ່ງຈາກ Use Case
/// </summary>
public interface IAuditableEntity
{
    /// <summary>ເວລາສ້າງຂໍ້ມູນ (timestamptz)</summary>
    DateTimeOffset CreatedAt { get; set; }

    /// <summary>ຜູ້ສ້າງຂໍ້ມູນແມ່ນ null ເມື່ອສ້າງໂດຍລະບົບ, ເຊັ່ນວ່າເມື່ອໃດ Seed</summary>
    Guid? CreatedBy { get; set; }

    /// <summary>ເວລາຂໍ້ມູນຖືກແກ້ໄຂຄັ້ງສຸດທ້າຍ</summary>
    DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>ບຸກຄົນທີ່ແກ້ໄຂຂໍ້ມູນຫຼ້າສຸດ</summary>
    Guid? UpdatedBy { get; set; }
}
