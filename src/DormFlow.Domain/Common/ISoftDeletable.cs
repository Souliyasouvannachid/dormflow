namespace DormFlow.Domain.Common;

/// <summary>
/// Entity ໃຊ້ Soft Delete ລະບົບບໍ່ໄດ້ລຶບແຖວ, ແຕ່ຕັ້ງພວກມັນ <see cref="IsDeleted"/> ແມ່ນ true
/// ແລະຖືກກັ່ນຕອງອອກໂດຍອັດຕະໂນມັດ Global Query Filter
/// </summary>
public interface ISoftDeletable
{
    /// <summary>ມັນຖືກລຶບແລ້ວບໍ?</summary>
    bool IsDeleted { get; set; }

    /// <summary>ເວລາທີ່ລຶບ</summary>
    DateTimeOffset? DeletedAt { get; set; }

    /// <summary>ບຸກຄົນທີ່ລຶບຂໍ້ມູນ</summary>
    Guid? DeletedBy { get; set; }
}
