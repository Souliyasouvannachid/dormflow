namespace DormFlow.Contracts.Parcels;

/// <summary>ການຮ້ອງຂໍເພື່ອບັນທຶກການຮັບການພັດທະນາ</summary>
public class PickupParcelRequest
{
    /// <summary>ຊື່ແທ້ຂອງຜູ້ທີ່ໄດ້ຮັບຊຸດ</summary>
    public string? PickedUpByName { get; set; }
}
