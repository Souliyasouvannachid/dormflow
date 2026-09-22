namespace DormFlow.Domain.Enums;

/// <summary>ສະຖານະຂອງອົງກອນທີ່ໃຊ້ລະບົບ</summary>
public enum OrganizationStatus
{
    /// <summary>ສາມາດໃຊ້ໄດ້ຕາມປົກກະຕິ</summary>
    Active = 1,

    /// <summary>ປິດການໃຊ້ງານຊົ່ວຄາວໂດຍອົງກອນເອງ</summary>
    Inactive = 2,

    /// <summary>ຖືກລະງັບໄວ້ໂດຍຜູ້ບໍລິຫານ ຜູ້ໃຊ້ໃນອົງກອນຂອງເຈົ້າຈະບໍ່ສາມາດເຂົ້າສູ່ລະບົບໄດ້</summary>
    Suspended = 3,
}
