namespace DormFlow.Contracts.Announcements;

/// <summary>ຂໍ້ມູນປະກາດຜຶ່ງ</summary>
public class AnnouncementResponse
{
    public string Priority { get; init; } = "Normal";
    public string[] TargetRoles { get; init; } = [];

    /// <summary>ລະຫັດອ້າງອິງປະກາດ</summary>
    public Guid Id { get; init; }

    /// <summary>ອົງກອນທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>ຫໍພັກທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ຫົວຂໍ້ປະກາດ</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>ເນື້ອໃນປະກາດ</summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>ສະຖານະປະກາດ</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>ເວລາເຜີຍແຜ່</summary>
    public DateTimeOffset? PublishedAt { get; init; }

    /// <summary>ປະກາດເວລາໝົດອາຍຸ</summary>
    public DateTimeOffset? ExpiresAt { get; init; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; init; }

    /// <summary>ເວລາສ້າງຂໍ້ມູນ</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
