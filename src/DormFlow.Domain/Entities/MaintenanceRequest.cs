using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ບົດລາຍງານການບໍາລຸງຮັກສາຜຶ່ງສໍາລັບຫ້ອງຜຶ່ງ
/// </summary>
/// <remarks>
/// ບໍ່ມີພາກສະຜາມຂໍ້ມູນຂ່າວສານແຍກຕ່າງຫາກເພາະວ່າ <see cref="Common.IAuditableEntity.CreatedBy"/> ທີ່ຖືກຕັ້ງອັດຕະໂນມັດ
/// ທຸກໆຄັ້ງທີ່ມັນຖືກບັນທຶກ, ເຈົ້າສາມາດບອກໄດ້ແລ້ວວ່າໃຜເປັນຜູ້ສ້າງບົດລາຍງານ
/// </remarks>
public class MaintenanceRequest : TenantEntity
{
    /// <summary>ລາຍງານຫ້ອງສໍາລັບການສ້ອມແປງ</summary>
    public Guid RoomId { get; set; }

    /// <summary>ຫົວຂໍ້ຂອງວຽກງານສ້ອມແປງ</summary>
    public required string Title { get; set; }

    /// <summary>ລາຍລະອຽດເພີ່ມເຕີມກ່ຽວກັບບັນຫາ</summary>
    public string? Description { get; set; }

    /// <summary>ລະດັບບູລິມະສິດ</summary>
    public MaintenancePriority Priority { get; set; } = MaintenancePriority.Medium;

    /// <summary>ສະຖານະການສ້ອມແປງ</summary>
    public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Reported;

    /// <summary>ນາຍຊ່າງການບຳລຸງຮັກສາທີ່ຖືກມອບໝາຍແມ່ນ null ໃນເວລາທີ່ມັນບໍ່ໄດ້ຖືກມອບຝາຍ</summary>
    public Guid? AssignedTechnicianId { get; set; }

    /// <summary>ເວລາຂອງການມອບໝາຍນັກວິຊາການຄັ້ງສຸດທ້າຍ</summary>
    public DateTimeOffset? AssignedAt { get; set; }

    /// <summary>ເວລາທີ່ການສ້ອມແປງໄດ້ຖືກສໍາເລັດ null ໃນເວລາທີ່ມັນບໍ່ສໍາເລັດ</summary>
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>ບັນທຶກຜົນການສ້ອມແປງ</summary>
    public string? ResolutionNotes { get; set; }
}
