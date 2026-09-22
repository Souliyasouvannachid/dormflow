namespace DormFlow.Contracts.Common;

/// <summary>
/// ຄ່າເລີ່ມຕົ້ນຂອງ pagination ທົ່ວລະບົບ ແລະຂອບເຂດ
/// </summary>
public static class PaginationDefaults
{
    /// <summary>ໝາຍເລກຜ້າທໍາອິດ</summary>
    public const int FirstPage = 1;

    /// <summary>ຈໍານວນລາຍການຕໍ່ຜ້າໃນເວລາທີ່ບໍ່ໄດ້ກໍານົດໂດຍຜູ້ໂທ</summary>
    public const int DefaultPageSize = 20;

    /// <summary>ຍອມຮັບຈໍານວນລາຍການຂັ້ນຕ່ໍາຕໍ່ຜ້າ</summary>
    public const int MinPageSize = 1;

    /// <summary>
    /// ຍອມຮັບຈໍານວນລາຍການສູງສຸດຕໍ່ຜ້າ
    /// ປ້ອງກັນການດຶງຂໍ້ມູນຕາຕະລາງທັງຝົດໃນຄໍາຮ້ອງຂໍດຽວແລະຜົນກະທົບຕໍ່ຖານຂໍ້ມູນ
    /// </summary>
    public const int MaxPageSize = 200;
}
