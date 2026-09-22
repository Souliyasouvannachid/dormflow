using DormFlow.Domain.Common;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ໃບຮັບເງິນຂອງຜຶ່ງລາຍການການຈ່າຍເງິນ
/// </summary>
/// <remarks>
/// ບໍ່ມີ Endpoint ແກ້ໄຂຫຼືລຶບໂດຍບັງເອີນ (ມີສິດເທົ່ານັ້ນ View ແລະ Issue) ເພາະຖືວ່າເປັນເອກະສານຫຼັກຖານ
/// ການຈ່າຍເງິນທີ່ຕ້ອງຄົງທີ່. ສາມາດອອກໄດ້ພຽງແຕ່ຄັ້ງດຽວຕໍ່ທຸລະກໍາການຈ່າຍເງິນ
/// ຈຳນວນທີ່ສຳເນົາມາຈາກ Payment ເມື່ອອອກໃບຮັບເງິນ ຢ່າອ້າງເຖິງຕົວແບບ Real-time
/// ດັ່ງນັ້ນເອກະສານທີ່ອອກແລ້ວຈະບໍ່ຖືກປ່ຽນແປງຄືນໃຝ່ເຖິງແມ່ນວ່າຂໍ້ມູນຕົ້ນສະບັບຈະຖືກດັດແກ້
/// </remarks>
public class Receipt : TenantEntity
{
    /// <summary>ບັນຊີລາຍຊື່ການຈ່າຍເງິນທີ່ໃບຮັບເງິນນີ້ອອກ</summary>
    public Guid PaymentId { get; set; }

    /// <summary>ໝາຍເລກໃບຮັບເງິນບໍ່ຊໍ້າກັນພາຍໃນອົງກອນດຽວກັນ</summary>
    public required string ReceiptNumber { get; set; }

    /// <summary>ຈຳນວນຕາມໃບຮັບເງິນທີ່ສຳເນົາມາຈາກ Payment ເມື່ອອອກໃບຮັບເງິນ</summary>
    public decimal Amount { get; set; }

    /// <summary>ເວລາອອກໃບຮັບເງິນ</summary>
    public DateTimeOffset IssuedAt { get; set; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
