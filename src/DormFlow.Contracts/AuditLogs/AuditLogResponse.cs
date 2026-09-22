namespace DormFlow.Contracts.AuditLogs;

/// <summary>ເຫດການໜຶ່ງທີ່ບັນທຶກໄວ້ໃນ Audit Log</summary>
public class AuditLogResponse
{
    /// <summary>ລະຫັດອ້າງອີງເຫດການ</summary>
    public Guid Id { get; init; }

    /// <summary>ອົງກອນທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid? OrganizationId { get; init; }

    /// <summary>ຫໍພັກທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid? DormitoryId { get; init; }

    /// <summary>ຜູ້ໃຊ້ທີ່ເຮັດໃຫ້ເກີດເຫດການ</summary>
    public Guid? UserId { get; init; }

    /// <summary>ປະເພດເຫດການ</summary>
    public string Action { get; init; } = string.Empty;

    /// <summary>ຊື່ Entity ໄດ້ເຮັດແລ້ວ</summary>
    public string EntityName { get; init; } = string.Empty;

    /// <summary>ລະຫັດຂອງ Entity ໄດ້ເຮັດແລ້ວ</summary>
    public string? EntityId { get; init; }

    /// <summary>ຄ່າຕົ້ນສະບັບກ່ອນການແກ້ໄຂ</summary>
    public string? OldValues { get; init; }

    /// <summary>ຄ່າໃຝ່ຫຼັງຈາກການແກ້ໄຂ</summary>
    public string? NewValues { get; init; }

    /// <summary>ເລກ IP ຕົ້ນກໍາເນີດ</summary>
    public string? IpAddress { get; init; }

    /// <summary>User Agent ຂອງຄໍາຮ້ອງສະຝັກ</summary>
    public string? UserAgent { get; init; }

    /// <summary>ເສັ້ນທາງຂອງການຮ້ອງຂໍທີ່ເຮັດໃຫ້ເກີດເຫດການ</summary>
    public string? RequestPath { get; init; }

    /// <summary>ເວລາຂອງເຫດການ</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
