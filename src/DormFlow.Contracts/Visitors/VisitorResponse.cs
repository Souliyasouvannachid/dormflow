namespace DormFlow.Contracts.Visitors;

/// <summary>ຂໍ້ມູນການເຂົ້າ-ແຂກຄົນຜຶ່ງອອກ</summary>
public class VisitorResponse
{
    /// <summary>ລະຫັດອ້າງອີງລາຍການ</summary>
    public Guid Id { get; init; }

    /// <summary>ອົງກອນທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>ຫໍພັກທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ຫ້ອງທີ່ນັກທ່ອງທ່ຽວມາຢ້ຽມຢາມ</summary>
    public Guid RoomId { get; init; }

    /// <summary>ຜູ້ເຊົ່າທີ່ມາຢ້ຽມຢາມ</summary>
    public Guid? TenantId { get; init; }

    /// <summary>ຊື່ນັກທ່ອງທ່ຽວ</summary>
    public string VisitorName { get; init; } = string.Empty;

    /// <summary>ເບີໂທລະສັບຂອງຜູ້ເຂົ້າຊົມ</summary>
    public string? VisitorPhone { get; init; }

    /// <summary>ບັດປະຈຳຕົວ ຫຼືໝາຍເລກໜັງສືຜ່ານແດນຂອງຜູ້ເຂົ້າຊົມ</summary>
    public string? IdNumber { get; init; }

    /// <summary>ຈຸດປະສົງຂອງການຢ້ຽມຢາມ</summary>
    public string? Purpose { get; init; }

    /// <summary>ເວລາເຂົ້າຫໍພັກ</summary>
    public DateTimeOffset CheckInAt { get; init; }

    /// <summary>ສະຖານະນັກທ່ອງທ່ຽວ</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>ເວລາອອກຈາກຫໍພັກ</summary>
    public DateTimeOffset? CheckOutAt { get; init; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; init; }

    /// <summary>ເວລາສ້າງຂໍ້ມູນ</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
