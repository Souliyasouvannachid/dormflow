using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>ອາຄານຫຼັງໜຶ່ງພາຍໃນຫໍພັກ</summary>
public class Building : TenantEntity
{
    /// <summary>ລະຫັດອາຄານແມ່ນເປັນເອກະລັກພາຍໃນຫໍພັກດຽວກັນ</summary>
    public required string Code { get; set; }

    /// <summary>ຊື່ອາຄານ</summary>
    public required string Name { get; set; }

    /// <summary>ຄໍາອະທິບາຍເພີ່ມເຕີມ</summary>
    public string? Description { get; set; }

    /// <summary>ຈໍານວນຊັ້ນທັງຝົດຂອງອາຄານ</summary>
    public int TotalFloors { get; set; } = 1;

    /// <summary>ສະຖານະຂອງອາຄານ</summary>
    public BuildingStatus Status { get; set; } = BuildingStatus.Active;
}
