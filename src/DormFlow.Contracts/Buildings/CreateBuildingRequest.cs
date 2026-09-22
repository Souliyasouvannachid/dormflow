namespace DormFlow.Contracts.Buildings;

/// <summary>ການຮ້ອງຂໍການກໍ່ສ້າງອາຄານໃຝ່</summary>
public class CreateBuildingRequest
{
    /// <summary>ຫໍພັກທີ່ອາຄານນີ້ຈະຂຶ້ນກັບ</summary>
    public Guid DormitoryId { get; set; }

    /// <summary>ລະຫັດອາຄານແມ່ນເປັນເອກະລັກພາຍໃນຫໍພັກດຽວກັນ</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>ຊື່ອາຄານ</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>ຄໍາອະທິບາຍເພີ່ມເຕີມ</summary>
    public string? Description { get; set; }

    /// <summary>ຈໍານວນຊັ້ນທັງຝົດຂອງອາຄານ</summary>
    public int TotalFloors { get; set; } = 1;
}
