namespace DormFlow.Contracts.Permissions;

/// <summary>ຂໍ້ມູນໃບອະນຸຍາດຜຶ່ງ</summary>
public class PermissionResponse
{
    /// <summary>ລະຫັດການອະນຸຍາດຖືກນໍາໃຊ້ເປັນເອກະສານອ້າງອີງໃນເວລາມອບຝາຍການອະນຸຍາດໃຫ້ກັບບົດບາດ</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>ສະແດງຊື່ການອະນຸຍາດ</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>ຄໍາອະທິບາຍເພີ່ມເຕີມ</summary>
    public string? Description { get; init; }

    /// <summary>ໂມດູນທີ່ສິດອະນຸຍາດນີ້ເປັນຂອງ</summary>
    public string Module { get; init; } = string.Empty;
}
