namespace DormFlow.Domain.Enums;

/// <summary>ສະຖານະການຈອງ</summary>
public enum ReservationStatus
{
    /// <summary>ລໍຖ້າໃຫ້ສໍາເລັດສັນຍາຫ້ອງຍັງຖືກປິດກັ້ນ</summary>
    Pending = 1,

    /// <summary>ມັນໄດ້ຖືກປ່ຽນເປັນສັນຍາເຊົ່າ</summary>
    ConvertedToContract = 2,

    /// <summary>ຍົກເລີກ</summary>
    Cancelled = 3,

    /// <summary>ຕັນຫ້ອງໄດ້ຝົດອາຍຸໂດຍບໍ່ມີການເຮັດສັນຍາ</summary>
    Expired = 4,
}
