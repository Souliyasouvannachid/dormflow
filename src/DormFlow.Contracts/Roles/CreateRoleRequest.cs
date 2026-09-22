namespace DormFlow.Contracts.Roles;

/// <summary>ຮຽກຮ້ອງໃຫ້ສ້າງບົດບາດໃຝ່</summary>
public class CreateRoleRequest
{
    /// <summary>ຊື່ບົດບາດແມ່ນເປັນເອກະລັກໃນທົ່ວລະບົບ</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>ລາຍລະອຽດບົດບາດ</summary>
    public string? Description { get; set; }
}
