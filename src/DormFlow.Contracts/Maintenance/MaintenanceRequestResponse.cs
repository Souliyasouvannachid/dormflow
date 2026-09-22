namespace DormFlow.Contracts.Maintenance;

/// <summary>ລາຍການໜຶ່ງຂອງຂໍ້ມູນວຽກບຳລຸງຮັກສາ</summary>
public class MaintenanceRequestResponse
{
    /// <summary>ສ້ອມແປງລະຫັດອ້າງອີງ</summary>
    public Guid Id { get; init; }

    /// <summary>ອົງກອນທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>ຫໍພັກທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ລາຍງານຫ້ອງສໍາລັບການສ້ອມແປງ</summary>
    public Guid RoomId { get; init; }

    /// <summary>ຫົວຂໍ້ຂອງວຽກງານສ້ອມແປງ</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>ລາຍລະອຽດເພີ່ມເຕີມກ່ຽວກັບບັນຫາ</summary>
    public string? Description { get; init; }

    /// <summary>ລະດັບບູລິມະສິດ</summary>
    public string Priority { get; init; } = string.Empty;

    /// <summary>ສະຖານະການສ້ອມແປງ</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>ຊ່າງສ້ອມແປງທີ່ຖືກແຕ່ງຕັ້ງ</summary>
    public Guid? AssignedTechnicianId { get; init; }

    /// <summary>ເວລາຂອງການມອບໝາຍນັກວິຊາການຄັ້ງສຸດທ້າຍ</summary>
    public DateTimeOffset? AssignedAt { get; init; }

    /// <summary>ເວລາຂອງການສໍາເລັດການສ້ອມແປງ</summary>
    public DateTimeOffset? CompletedAt { get; init; }

    /// <summary>ບັນທຶກຜົນການສ້ອມແປງ</summary>
    public string? ResolutionNotes { get; init; }

    /// <summary>ເວລາສ້າງຂໍ້ມູນ</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
