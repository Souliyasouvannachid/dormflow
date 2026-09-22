namespace DormFlow.Application.Common.Authorization;

/// <summary>
/// ຄໍານິຍາມຂອງຜຶ່ງສິດນໍາໃຊ້ມັນເປັນແຫຼ່ງດຽວຂອງຄວາມຈິງສໍາລັບ Seed ໃສ່ຖານຂໍ້ມູນ
/// </summary>
/// <param name="Code">ລະຫັດການອະນຸຍາດທີ່ໃຊ້ໃນການກວດສອບລະຫັດແລະໃນ Claim</param>
/// <param name="Name">ສະແດງຊື່</param>
/// <param name="Module">ໂມດູນທີ່ສິດອະນຸຍາດນີ້ເປັນຂອງ</param>
/// <param name="Description">ຄໍາອະທິບາຍເພີ່ມເຕີມ</param>
public sealed record PermissionDefinition(
    string Code,
    string Name,
    string Module,
    string? Description = null);
