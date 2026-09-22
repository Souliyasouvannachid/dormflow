namespace DormFlow.Contracts.Dormitories;

/// <summary>ຂໍ້ມູນຫໍພັກຜຶ່ງ</summary>
public class DormitoryResponse
{
    /// <summary>ລະຫັດອ້າງອີງຫໍພັກ</summary>
    public Guid Id { get; init; }

    /// <summary>ອົງກອນເຈົ້າຂອງຫໍພັກ</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>ລະຫັດຫໍພັກ</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>ຊື່ຫໍພັກ</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>ຄໍາອະທິບາຍເພີ່ມເຕີມ</summary>
    public string? Description { get; init; }

    /// <summary>ເບີໂທລະສັບຕິດຕໍ່</summary>
    public string? Phone { get; init; }

    /// <summary>ອີເມວຕິດຕໍ່</summary>
    public string? Email { get; init; }

    /// <summary>ທີ່ຢູ່</summary>
    public string? Address { get; init; }

    /// <summary>ແຂວງ</summary>
    public string? Province { get; init; }

    /// <summary>ເມືອງ/ພື້ນທີ່</summary>
    public string? District { get; init; }

    /// <summary>ເມືອງ/ເມືອງ</summary>
    public string? Subdistrict { get; init; }

    /// <summary>ລະຫັດໄປສະນີ</summary>
    public string? PostalCode { get; init; }

    /// <summary>ເສັ້ນຂະໜານ</summary>
    public decimal? Latitude { get; init; }

    /// <summary>ເສັ້ນແວງ</summary>
    public decimal? Longitude { get; init; }

    /// <summary>ວັນທີໃນແຕ່ລະເດືອນໃບແຈ້ງໜີ້ຖືກອອກ</summary>
    public int BillingDay { get; init; }

    /// <summary>ວັນທີການຊໍາລະເງິນໃນແຕ່ລະເດືອນ</summary>
    public int PaymentDueDay { get; init; }

    /// <summary>ຮູບແບບການຄິດໄລ່ຄ່າທຳນຽມການຈ່າຍເງິນຊ້າ</summary>
    public string LateFeeType { get; init; } = string.Empty;

    /// <summary>ຈໍານວນການລົງໂທດການຈ່າຍເງິນຊ້າ</summary>
    public decimal LateFeeAmount { get; init; }

    /// <summary>ອັດຕານໍ້າຕໍ່ຫົວໜ່ວຍ</summary>
    public decimal WaterRatePerUnit { get; init; }

    /// <summary>ອັດຕາຄ່າໄຟຟ້າຕໍ່ຫົວໜ່ວຍ</summary>
    public decimal ElectricityRatePerUnit { get; init; }

    /// <summary>ສະຖານະຫໍພັກ</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>ເວລາສ້າງຂໍ້ມູນ</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>ເວລາຂອງການດັດແກ້ຫຼ້າສຸດ</summary>
    public DateTimeOffset? UpdatedAt { get; init; }
}
