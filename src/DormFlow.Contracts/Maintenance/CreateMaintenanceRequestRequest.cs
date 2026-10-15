namespace DormFlow.Contracts.Maintenance;

/// <summary>ຮ້ອງຂໍໃຫ້ມີການແຈ້ງເຕືອນການບໍາລຸງຮັກສາ</summary>
public class CreateMaintenanceRequestRequest
{
    public Guid? TenantId { get; set; }
    public string Category { get; set; } = "General";

    /// <summary>ລາຍງານຫ້ອງສໍາລັບການສ້ອມແປງ</summary>
    public Guid RoomId { get; set; }

    /// <summary>ຫົວຂໍ້ຂອງວຽກງານສ້ອມແປງ</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>ລາຍລະອຽດເພີ່ມເຕີມກ່ຽວກັບບັນຫາ</summary>
    public string? Description { get; set; }

    /// <summary>ລະດັບບູລິມະສິດ ("Low", "Medium", "High", "Urgent")</summary>
    public string Priority { get; set; } = "Medium";
}
