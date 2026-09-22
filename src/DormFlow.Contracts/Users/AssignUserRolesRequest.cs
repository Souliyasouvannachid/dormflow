namespace DormFlow.Contracts.Users;

/// <summary>ການຮ້ອງຂໍການມອບຝາຍບົດບາດຂອງຜູ້ໃຊ້</summary>
/// <remarks>ລາຍຊື່ບົດບາດທີ່ສົ່ງມາແທນບົດບາດຕົ້ນສະບັບທັງໝົດຂອງຜູ້ໃຊ້. ມັນບໍ່ແມ່ນການເພີ່ມເຕີມ</remarks>
public class AssignUserRolesRequest
{
    /// <summary>ລາຍຊື່ບົດບາດທີ່ທ່ານຕ້ອງການມອບໝາຍໃຫ້ຜູ້ໃຊ້</summary>
    public IReadOnlyList<string> Roles { get; set; } = [];
}
