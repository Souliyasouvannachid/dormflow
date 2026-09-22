namespace DormFlow.Domain.Entities;

/// <summary>
/// ການມອບຝາຍໃຫ້ຜູ້ໃຊ້ຈັດການຜຶ່ງຫໍພັກ
/// </summary>
/// <remarks>
/// ຜູ້ໃຊ້ຄົນດຽວສາມາດເບິ່ງແຍງຫໍພັກຫຼາຍບ່ອນ. ແລະຜຶ່ງຫໍພັກສາມາດມີຜູ້ບໍລິຫານຈໍານວນຫຼາຍ
/// ໃຊ້ Id ເປັນຂອງຕົນເອງແທນ Composite Key ມີຖັນ assigned_at ແລະ assigned_by ແຕ່ລະວຽກທີ່ຈະແຈ້ງ
/// ຢ່າໃຊ້ Soft Delete ເນື່ອງຈາກວ່າການຖອນສິດໃນການດູແລຂອງຫໍພັກໃນຕົວຈິງແລ້ວຄວນຈະລົບແຖວ
/// </remarks>
public class UserDormitory
{
    /// <summary>ລະຫັດອ້າງອີງການມອບໝາຍ</summary>
    public Guid Id { get; init; } = Guid.CreateVersion7();

    /// <summary>ຜູ້ໃຊ້ທີ່ຖືກມອບຝາຍ</summary>
    public Guid UserId { get; set; }

    /// <summary>ຫໍພັກທີ່ຖືກມອບໝາຍ</summary>
    public Guid DormitoryId { get; set; }

    /// <summary>ນີ້ແມ່ນຫໍພັກຫຼັກຂອງຜູ້ໃຊ້ບໍ?</summary>
    public bool IsPrimary { get; set; }

    /// <summary>ເວລາມອບໝາຍ</summary>
    public DateTimeOffset AssignedAt { get; set; }

    /// <summary>ຜູ້ມອບໝາຍ</summary>
    public Guid? AssignedBy { get; set; }
}
