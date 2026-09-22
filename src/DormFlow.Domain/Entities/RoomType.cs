using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>ປະເພດຫ້ອງໜຶ່ງພາຍໃນຫໍພັກ ເຊັ່ນ: ຫ້ອງແອ ແລະ ຫ້ອງພັດລົມ</summary>
public class RoomType : TenantEntity
{
    /// <summary>ລະຫັດປະເພດຫ້ອງບໍ່ຊໍ້າກັນພາຍໃນຫໍພັກດຽວກັນ</summary>
    public required string Code { get; set; }

    /// <summary>ຊື່ປະເພດຫ້ອງ</summary>
    public required string Name { get; set; }

    /// <summary>ຄໍາອະທິບາຍເພີ່ມເຕີມ</summary>
    public string? Description { get; set; }

    /// <summary>ຄ່າເຊົ່າລາຍເດືອນ</summary>
    public decimal MonthlyRent { get; set; }

    /// <summary>ຄ່າເຊົ່າປະຈໍາວັນແມ່ນ null ເມື່ອຫ້ອງປະເພດນີ້ບໍ່ຮອງຮັບການເຊົ່າປະຈໍາວັນ</summary>
    public decimal? DailyRent { get; set; }

    /// <summary>ຝາກ</summary>
    public decimal DepositAmount { get; set; }

    /// <summary>ຈຳນວນແຂກສູງສຸດ</summary>
    public int MaximumOccupants { get; set; } = 1;

    /// <summary>ສະຖານະປະເພດຫ້ອງ</summary>
    public RoomTypeStatus Status { get; set; } = RoomTypeStatus.Active;
}
