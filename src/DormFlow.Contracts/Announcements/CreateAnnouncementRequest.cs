namespace DormFlow.Contracts.Announcements;

/// <summary>ຮ້ອງຂໍເພື່ອສ້າງການປະກາດໃຝ່ (ສ້າງຢູ່ໃນສະຖານະສະບັບຮ່າງສະເໝີ)</summary>
public class CreateAnnouncementRequest
{
    /// <summary>ຫໍພັກທີ່ປະກາດນີ້ກ່ຽວຂ້ອງ</summary>
    public Guid DormitoryId { get; set; }

    /// <summary>ຫົວຂໍ້ປະກາດ</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>ເນື້ອໃນປະກາດ</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>ເວລາປະກາດໝົດອາຍຸບໍ່ໄດ້ລະບຸ</summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
