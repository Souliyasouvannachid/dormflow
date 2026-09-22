namespace DormFlow.Domain.Enums;

/// <summary>ສະຖານະຫ້ອງ</summary>
public enum RoomStatus
{
    /// <summary>ວ່າງ, ພ້ອມໃຫ້ເຊົ່າ</summary>
    Available = 1,

    /// <summary>ຈອງແລ້ວ ບໍ່ໄດ້ຢູ່</summary>
    Reserved = 2,

    /// <summary>ມີຄົນເຊົ່າ</summary>
    Occupied = 3,

    /// <summary>ປະຈຸບັນທໍາຄວາມສະອາດກ່ອນການເຊົ່າຕໍ່ໄປ</summary>
    Cleaning = 4,

    /// <summary>ພາຍໃຕ້ການບໍາລຸງຮັກສາບໍ່ມີການຊົ່ວຄາວສໍາລັບການເຊົ່າ</summary>
    Maintenance = 5,

    /// <summary>ຄົນພິການ, ບໍ່ມີໃຫ້ເຊົ່າ</summary>
    Inactive = 6,
}
