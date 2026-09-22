namespace DormFlow.Contracts.Users;

/// <summary>ຮ້ອງຂໍມອບຝາຍໃຫ້ຜູ້ໃຊ້ໃນຫໍພັກ</summary>
public class AssignUserDormitoryRequest
{
    /// <summary>ຫໍພັກທີ່ຈະມອບຝາຍ</summary>
    public Guid DormitoryId { get; set; }

    /// <summary>ຕັ້ງຫໍພັກຂອງຜູ້ໃຊ້ນີ້ເປັນຫໍພັກຫຼັກບໍ?</summary>
    public bool IsPrimary { get; set; }
}
