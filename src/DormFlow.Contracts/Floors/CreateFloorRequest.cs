namespace DormFlow.Contracts.Floors;

/// <summary>ຮ້ອງຂໍການສ້າງຊັ້ນໃຝ່</summary>
public class CreateFloorRequest
{
    /// <summary>ອາຄານທີ່ຊັ້ນນີ້ຂຶ້ນກັບ</summary>
    public Guid BuildingId { get; set; }

    /// <summary>ລໍາດັບຊັ້ນແມ່ນເປັນເອກະລັກພາຍໃນອາຄານດຽວກັນ</summary>
    public int FloorNumber { get; set; }

    /// <summary>ຊື່ຊັ້ນສໍາລັບການສະແດງ, ເຊັ່ນ: "ຊັ້ນ G"</summary>
    public string? Name { get; set; }

    /// <summary>ຄໍາອະທິບາຍເພີ່ມເຕີມ</summary>
    public string? Description { get; set; }
}
