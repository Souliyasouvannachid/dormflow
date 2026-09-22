namespace DormFlow.Domain.Enums;

/// <summary>ສະຖານະຂອງວຽກງານບໍາລຸງຮັກສາ</summary>
public enum MaintenanceStatus
{
    /// <summary>ການສ້ອມແປງໄດ້ຖືກລາຍງານແລ້ວ, ແຕ່ຍັງບໍ່ມີນາຍຊ່າງໃດໄດ້ຮັບການແຕ່ງຕັ້ງ</summary>
    Reported = 1,

    /// <summary>ຊ່າງສ້ອມແປງໄດ້ຖືກມອບໝາຍໃຫ້</summary>
    Assigned = 2,

    /// <summary>ກຳລັງສ້ອມແປງຢູ່</summary>
    InProgress = 3,

    /// <summary>ການສ້ອມແປງສໍາເລັດ</summary>
    Completed = 4,

    /// <summary>ຍົກເລີກ</summary>
    Cancelled = 5,
}
