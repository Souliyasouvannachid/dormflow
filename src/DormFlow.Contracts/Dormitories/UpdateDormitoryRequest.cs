namespace DormFlow.Contracts.Dormitories;

/// <summary>ຮ້ອງຂໍແກ້ໄຂຂໍ້ມູນຫໍພັກ</summary>
/// <remarks>ບໍ່ມີພາກສະຜາມ OrganizationId ເນື່ອງຈາກວ່າການຍ້າຍຫໍພັກໃນທົ່ວອົງກອນບໍ່ແມ່ນການປ່ຽນແປງຂໍ້ມູນປົກກະຕິ</remarks>
public class UpdateDormitoryRequest
{
    /// <summary>ລະຫັດຫໍພັກແມ່ນເປັນເອກະລັກພາຍໃນອົງກອນດຽວກັນ</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>ຊື່ຫໍພັກ</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>ຄໍາອະທິບາຍເພີ່ມເຕີມ</summary>
    public string? Description { get; set; }

    /// <summary>ເບີໂທລະສັບຕິດຕໍ່</summary>
    public string? Phone { get; set; }

    /// <summary>ອີເມວຕິດຕໍ່</summary>
    public string? Email { get; set; }

    /// <summary>ທີ່ຢູ່</summary>
    public string? Address { get; set; }

    /// <summary>ແຂວງ</summary>
    public string? Province { get; set; }

    /// <summary>ເມືອງ/ພື້ນທີ່</summary>
    public string? District { get; set; }

    /// <summary>ເມືອງ/ເມືອງ</summary>
    public string? Subdistrict { get; set; }

    /// <summary>ລະຫັດໄປສະນີ</summary>
    public string? PostalCode { get; set; }

    /// <summary>ເສັ້ນຂະໜານ</summary>
    public decimal? Latitude { get; set; }

    /// <summary>ເສັ້ນແວງ</summary>
    public decimal? Longitude { get; set; }

    /// <summary>ວັນທີໃນແຕ່ລະເດືອນໃບແຈ້ງໜີ້ຖືກອອກ (1-28)</summary>
    public int BillingDay { get; set; } = 1;

    /// <summary>ວັນທີການຊໍາລະເງິນໃນແຕ່ລະເດືອນ (1-31)</summary>
    public int PaymentDueDay { get; set; } = 5;

    /// <summary>ຮູບແບບການຄິດໄລ່ຄ່າທຳນຽມການຈ່າຍເງິນຊ້າ</summary>
    public string LateFeeType { get; set; } = "Fixed";

    /// <summary>ຈໍານວນການລົງໂທດການຈ່າຍເງິນຊ້າ</summary>
    public decimal LateFeeAmount { get; set; }

    /// <summary>ອັດຕາຄ່ານ້ຳຕໍ່ຫົວໜ່ວຍ ໃຊ້ເພື່ອຄິດໄລ່ຄ່ານ້ຳໃນໃບແຈ້ງໜີ້ຈາກຄ່າແມັດ</summary>
    public decimal WaterRatePerUnit { get; set; }

    /// <summary>ອັດຕາຄ່າໄຟຟ້າຕໍ່ຫົວໜ່ວຍໃຊ້ເພື່ອຄິດໄລ່ຄ່າໄຟຟ້າໃນໃບແຈ້ງໜີ້ຈາກຄ່າແມັດ</summary>
    public decimal ElectricityRatePerUnit { get; set; }
}
