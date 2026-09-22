namespace DormFlow.Contracts.Buildings;

/// <summary>ຮ້ອງຂໍການແກ້ໄຂຂໍ້ມູນອາຄານ</summary>
/// <remarks>ບໍ່ມີພາກສະຜາມ DormitoryId ເນື່ອງຈາກວ່າການຍ້າຍອາຄານໃນທົ່ວຫໍພັກບໍ່ແມ່ນການແກ້ໄຂຂໍ້ມູນປົກກະຕິ</remarks>
public class UpdateBuildingRequest
{
    /// <summary>ລະຫັດອາຄານແມ່ນເປັນເອກະລັກພາຍໃນຫໍພັກດຽວກັນ</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>ຊື່ອາຄານ</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>ຄໍາອະທິບາຍເພີ່ມເຕີມ</summary>
    public string? Description { get; set; }

    /// <summary>ຈໍານວນຊັ້ນທັງຝົດຂອງອາຄານ</summary>
    public int TotalFloors { get; set; } = 1;
}
