using Microsoft.AspNetCore.Authorization;

namespace DormFlow.Api.Authorization;

/// <summary>
/// ຕົກແຕ່ງ Endpoint ທີ່ຮຽກຮ້ອງໃຫ້ມີສິດສະເພາະ
/// </summary>
/// <remarks>
/// ໃຊ້ແທນ [Authorize(Policy = "...")] ໂດຍກົງເພື່ອອ່ານລະຫັດ Controller ເຂົ້າໃຈງ່າຍກວ່າ
/// ແລະໃຫ້ຊື່ Policy ແທ້ຈິງແມ່ນຜະລິດຈາກລະຫັດການອະນຸຍາດອັດຕະໂນມັດຢູ່ໃນສະຖານທີ່ດຽວ
/// </remarks>
public sealed class RequirePermissionAttribute : AuthorizeAttribute
{
    /// <summary>ຄໍານໍາຜ້າຂອງທຸກໆ Policy ສ້າງຈາກການອະນຸຍາດ, ໃຊ້ແຍກຕ່າງຫາກຈາກ Policy ປະເພດອື່ນໆ</summary>
    public const string PolicyPrefix = "Permission:";

    /// <summary>ສ້າງ Attribute ຈາກລະຫັດການອະນຸຍາດທີ່ຕ້ອງການ</summary>
    /// <param name="permissionCode">ລະຫັດການອະນຸຍາດເຊັ່ນ rooms.create</param>
    public RequirePermissionAttribute(string permissionCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permissionCode);
        Policy = PolicyPrefix + permissionCode;
    }
}
