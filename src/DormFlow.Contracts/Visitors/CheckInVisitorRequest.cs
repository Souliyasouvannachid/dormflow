namespace DormFlow.Contracts.Visitors;

/// <summary>ຄຳຮ້ອງຂໍບັນທຶກຜູ້ເຂົ້າຊົມ</summary>
public class CheckInVisitorRequest
{
    /// <summary>ຫ້ອງທີ່ນັກທ່ອງທ່ຽວມາຢ້ຽມຢາມ</summary>
    public Guid RoomId { get; set; }

    /// <summary>ຜູ້ເຊົ່າທີ່ເຂົ້າມາຢ້ຽມຢາມແມ່ນ null ເມື່ອບໍ່ໄດ້ລະບຸ</summary>
    public Guid? TenantId { get; set; }

    /// <summary>ຊື່ນັກທ່ອງທ່ຽວ</summary>
    public string VisitorName { get; set; } = string.Empty;

    /// <summary>ເບີໂທລະສັບຂອງຜູ້ເຂົ້າຊົມ</summary>
    public string? VisitorPhone { get; set; }

    /// <summary>ບັດປະຈຳຕົວ ຫຼືໝາຍເລກໜັງສືຜ່ານແດນຂອງຜູ້ເຂົ້າຊົມ</summary>
    public string? IdNumber { get; set; }

    /// <summary>ຈຸດປະສົງຂອງການຢ້ຽມຢາມ</summary>
    public string? Purpose { get; set; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
