using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>ຊັ້ນທໍາອິດຂອງອາຄານ</summary>
public class Floor : TenantEntity
{
    /// <summary>ອາຄານທີ່ຊັ້ນນີ້ຂຶ້ນກັບ</summary>
    public Guid BuildingId { get; set; }

    /// <summary>ລຳດັບ</summary>
    public int FloorNumber { get; set; }

    /// <summary>ຊື່ຊັ້ນສໍາລັບການສະແດງ, ເຊັ່ນ: "ຊັ້ນ G"</summary>
    public string? Name { get; set; }

    /// <summary>ຄໍາອະທິບາຍເພີ່ມເຕີມ</summary>
    public string? Description { get; set; }

    /// <summary>ສະຖານະຊັ້ນ</summary>
    public FloorStatus Status { get; set; } = FloorStatus.Active;
}
