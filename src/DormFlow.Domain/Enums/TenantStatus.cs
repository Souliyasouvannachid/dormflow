namespace DormFlow.Domain.Enums;

/// <summary>ສະຖານະຜູ້ເຊົ່າ</summary>
public enum TenantStatus
{
    /// <summary>ສາມາດໃຊ້ໄດ້ຕາມປົກກະຕິ</summary>
    Active = 1,

    /// <summary>ປິດການໃຊ້ງານຊົ່ວຄາວ, ສໍາລັບການຍົກຕົວຢ່າງ, ບໍ່ມີສັນຍາທີ່ມີການເຄື່ອນໄຫວ</summary>
    Inactive = 2,

    /// <summary>ບັນຊີດໍາ, ຫ້າມບໍ່ໃຫ້ເຮັດສັນຍາເຊົ່າໃຝ່</summary>
    Blacklisted = 3,
}
