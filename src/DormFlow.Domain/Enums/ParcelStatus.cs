namespace DormFlow.Domain.Enums;

/// <summary>ສະຖານະການຫຸ້ມຫໍ່</summary>
public enum ParcelStatus
{
    /// <summary>ຊຸດໄດ້ຖືກຮັບຢູ່ຫໍພັກ. ລໍຖ້າໃຫ້ຜູ້ເຊົ່າມາຮັບ</summary>
    Received = 1,

    /// <summary>ຜູ້ເຊົ່າໄດ້ຮັບຊຸດແລ້ວ</summary>
    PickedUp = 2,
    Returned = 3,
}
