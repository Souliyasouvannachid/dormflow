namespace DormFlow.Domain.Enums;

/// <summary>ຮູບແບບການຄິດໄລ່ຄ່າທຳນຽມການຊຳລະຊ້າຂອງຫໍພັກ</summary>
public enum LateFeeType
{
    /// <summary>ຄິດໄລ່ເປັນຈໍານວນຄົງທີ່</summary>
    Fixed = 1,

    /// <summary>ຄິດໄລ່ເປັນເປີເຊັນຂອງຍອດຄົງຄ້າງ</summary>
    Percentage = 2,
}
