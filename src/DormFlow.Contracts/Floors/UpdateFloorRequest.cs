namespace DormFlow.Contracts.Floors;

/// <summary>ຮ້ອງຂໍການແກ້ໄຂຂໍ້ມູນຊັ້ນ</summary>
public class UpdateFloorRequest
{
    /// <summary>ລໍາດັບຊັ້ນແມ່ນເປັນເອກະລັກພາຍໃນອາຄານດຽວກັນ</summary>
    public int FloorNumber { get; set; }

    /// <summary>ຊື່ຫ້ອງຮຽນສໍາລັບການສະແດງ</summary>
    public string? Name { get; set; }

    /// <summary>ຄໍາອະທິບາຍເພີ່ມເຕີມ</summary>
    public string? Description { get; set; }
}
