using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ຄ່າໃຊ້ຈ່າຍແມັດຂອງຫ້ອງຜຶ່ງໃນໄລຍະໃບບິນ
/// </summary>
/// <remarks>
/// ບໍ່ມີ Endpoint ແກ້ໄຂຫຼືລຶບໂດຍບັງເອີນ (ມີສິດເທົ່ານັ້ນ View ແລະ Create)
/// ເພາະຖືວ່າເປັນບັນທຶກປະຫວັດສາດທີ່ໃຊ້ໃນການຄິດໄລ່ຄ່ານ້ຳ-ແສງໃບແຈ້ງໜີ້ ຖ້າເຈົ້າເຮັດຜິດ, ເຈົ້າຕ້ອງເຮັດລາຍການໃໝ່ເພື່ອແກ້ໄຂມັນ
/// </remarks>
public class MeterReading : TenantEntity
{
    /// <summary>ຫ້ອງທີ່ບັນທຶກການອ່ານແມັດ</summary>
    public Guid RoomId { get; set; }

    /// <summary>ປະເພດແມັດ</summary>
    public MeterType MeterType { get; set; }

    /// <summary>ເດືອນທີ່ຄ່ານີ້ຖືກຮຽກເກັບປັບເປັນວັນທີ 1 ປະຈຳເດືອນ</summary>
    public DateOnly PeriodMonth { get; set; }

    /// <summary>ວັນທີລົງທະບຽນຕົວຈິງແມັດ</summary>
    public DateOnly ReadingDate { get; set; }

    /// <summary>ຄ່າແມັດກ່ອນ ສຳເນົາມາຈາກຄ່າຫ້ອງຫຼ້າສຸດ+ປະເພດນີ້ແມ່ນອັດຕະໂນມັດ</summary>
    public decimal PreviousReading { get; set; }

    /// <summary>ຄ່າແມັດທີ່ບັນທຶກໄວ້ໃນຮອບນີ້</summary>
    public decimal CurrentReading { get; set; }

    /// <summary>ຫົວໜ່ວຍທີ່ໃຊ້ການຄິດໄລ່ ແລະເກັບຮັກສາໃນເວລາບັນທຶກ (CurrentReading - PreviousReading)</summary>
    public decimal UnitsUsed { get; set; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
