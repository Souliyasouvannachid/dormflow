namespace DormFlow.Domain.Enums;

/// <summary>ສະຖານະຂອງສັນຍາເຊົ່າ</summary>
public enum ContractStatus
{
    /// <summary>ຮ່າງສັນຍາດັ່ງກ່າວຍັງບໍ່ທັນໄດ້ຮັບການເປີດໃຊ້ເທື່ອ. ຫ້ອງໄດ້ຖືກສະຫງວນໄວ້ສໍາລັບການຢູ່</summary>
    Draft = 1,

    /// <summary>ເປີດໃຊ້ແລ້ວ ຜູ້ເຊົ່າກຳລັງພັກຢູ່</summary>
    Active = 2,

    /// <summary>ສັນຍາໄດ້ສິ້ນສຸດລົງ. ຜູ້ເຊົ່າໄດ້ຍ້າຍອອກ</summary>
    Terminated = 3,

    /// <summary>ໝົດອາຍຸຕາມທີ່ລະບຸໄວ້ໃນສັນຍາໂດຍບໍ່ມີການຕໍ່ອາຍຸ</summary>
    Expired = 4,
}
