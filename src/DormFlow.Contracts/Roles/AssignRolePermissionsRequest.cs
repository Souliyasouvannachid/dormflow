namespace DormFlow.Contracts.Roles;

/// <summary>ຮ້ອງຂໍໃຫ້ມອບສິດໃຫ້ບົດບາດ</summary>
/// <remarks>ລາຍຊື່ສິດອະນຸຍາດທີ່ສົ່ງມາຈະລົບລ້າງສິດຕົ້ນສະບັບທັງໝົດຂອງບົດບາດ. ມັນບໍ່ແມ່ນການເພີ່ມເຕີມ</remarks>
public class AssignRolePermissionsRequest
{
    /// <summary>ລະຫັດການອະນຸຍາດເພື່ອມອບໝາຍໜ້າທີ່ນີ້</summary>
    public IReadOnlyList<string> Permissions { get; set; } = [];
}
